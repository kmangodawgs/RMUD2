﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public static class Network
{

    static HttpListener? httpListener;

    static Task receiverTask;
    static Task[] handlerTasks = new Task[Config.HANDLER_THREADS];

    static List<HttpListenerContext> requests = new();

    public static void Init()
    {
        Console.WriteLine("Initializing Network...");

        httpListener = new HttpListener();

        string prefix = "http://localhost:" + Config.PORT + "/";
        Console.WriteLine("Adding prefix: " + prefix);
        httpListener.Prefixes.Add(prefix);

        httpListener.Start();

        receiverTask = Task.Run(Start);

        for(int i = 0; i < handlerTasks.Length; i++)
        {
            int id = i;
            handlerTasks[i] = Task.Run(()=>HandlerThread(id));
        }

        Console.WriteLine("Network initialized");
    }

    static void Start()
    {
        Console.WriteLine("Started Network thread");

        if (httpListener != null)
        {
            while (true)
            {
                //GetContext() waits until we receive a connection
                requests.Add(httpListener.GetContext());
            }
        }

        Console.WriteLine("Stopping Network thread");
    }

    static void HandlerThread(int id)
    {
        while (true)
        {
            if (requests.Count > 0)
            {
                Console.WriteLine("Request received by handler " + id);
                HttpListenerContext ctx = requests[0];

                requests.RemoveAt(0);
                HandleRequest(ctx);
            }

            Thread.Sleep(Config.HANDLER_SLEEP_INTERVAL);
        }
    }

    static void HandleRequest(HttpListenerContext ctx)
    {
        HttpListenerRequest req = ctx.Request;
        HttpListenerResponse resp = ctx.Response;

        Console.WriteLine("Received HTTP request. Method: " + req.HttpMethod + ", Body: ");

        resp.StatusCode = (int)HttpStatusCode.OK;
        resp.StatusDescription = "Status OK";

        //From https://stackoverflow.com/a/28223114, prevents CORS errors on client
        if (req.HttpMethod.Equals("Options"))
        {
            resp.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
            resp.AddHeader("Access-Control-Allow-Methods", "GET, POST");
            resp.AddHeader("Access-Control-Max-Age", "1728000");
        }
        resp.AppendHeader("Access-Control-Allow-Origin", "*");

        resp.Headers.Set("Content-Type", "text/plain");

        //Read body
        string body = new StreamReader(ctx.Request.InputStream).ReadToEnd();
        Console.WriteLine(body);

        string response = "";

        //Parse body
        ClientAction action = JsonConvert.DeserializeObject<ClientAction>(body);

        //Write response
        Console.WriteLine("Writing response: " + response);
        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
        resp.ContentLength64 = responseBytes.Length;
        resp.OutputStream.Write(responseBytes, 0, responseBytes.Length);

        resp.Close();
    }

}
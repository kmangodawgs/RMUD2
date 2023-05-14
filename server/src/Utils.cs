﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public static class Utils
{

    /// <summary>
    /// Creates a random salt that is cryptographically secure
    /// </summary>
    /// <returns>Returns the salt</returns>
    public static string RandomSalt()
    {
        byte[] bytes = new byte[32];
        RandomNumberGenerator.Create().GetBytes(bytes); //Fills out the byte array with random values

        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// We use this instead of Sha256. It's more secure
    /// </summary>
    /// <param name="text">The password to hash</param>
    /// <param name="salt">The salt</param>
    /// <returns>The PBKDF2 hash</returns>
    public static string PBKDF2Hash(string text, string salt)
    {
        Rfc2898DeriveBytes pbkdf2 = new(text, Encoding.UTF8.GetBytes(salt));
        pbkdf2.IterationCount = Config.PBKDF2_ITERATIONS;
        return Convert.ToBase64String(pbkdf2.GetBytes(32));
    }

    public static string Style(string text, string color = "", bool bold = false, bool underline = false)
    {
        if (!color.Equals(""))
            text = "<p style=" + '"' + "color:" + color + ";" + '"' + ">" + text + "</p>";
        if (bold) text = "<b>" + text + "</b>";
        if (underline) text = "<u>" + text + "</u>";

        return text;
    }

    /// <summary>
    /// Checks if the input contains malicious text. NOT IMPLEMENTED YET
    /// </summary>
    /// <param name="text">The text to scan</param>
    /// <returns>Whether the text is safe or not</returns>
    public static bool IsInputSafe(string text)
    {
        return true;
    }

}
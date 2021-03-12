﻿namespace NServiceBus.RavenDB.Tests
{
    using System;

    class TestConstants
    {
        public static string[] RavenUrls
        {
            get
            {
                var urls = Environment.GetEnvironmentVariable("CommaSeparatedRavenClusterUrls") ?? "http://localhost:8080";
                return urls.Split(',');
            }
        }
    }
}

﻿namespace NServiceBus.RavenDB.Persistence.SubscriptionStorage
{
    using System;

    class SubscriptionClient
    {
        public string TransportAddress { get; set; }

        public string Endpoint { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is SubscriptionClient client && Equals(client);
        }

        bool Equals(SubscriptionClient obj) =>
            string.Equals(TransportAddress, obj.TransportAddress, StringComparison.InvariantCultureIgnoreCase);

        public override int GetHashCode() => TransportAddress.ToLowerInvariant().GetHashCode();
    }
}
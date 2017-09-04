// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;

namespace IdentityServer4.EntityFramework.Entities
{
    public class PersistedGrant
    {
        public string Key { get; set; }
        public string Type { get; set; }

        string _subjectId;
        public string SubjectId
        {
            get => _subjectId;
            set
            {
                NormalizedSubjectId = (_subjectId = value)?.Normalize();
            }
        }
        public string NormalizedSubjectId { get; set; }

        string _clientId;
        public string ClientId
        {
            get => _clientId;
            set
            {
                NormalizedClientId = (_clientId = value)?.Normalize();
            }
        }
        public string NormalizedClientId { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime? Expiration { get; set; }
        public string Data { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Client
{
    public static class Extensions
    {
        public static void AddSecurityToken(this WebRequest request,  string token)
        {
            request.Headers.Add("token",Hash.GetHash(request.RequestUri.ToString().ToLowerInvariant(),token,HashType.SHA512,new UTF8Encoding()));
        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AutismHelper.Models
{
    public class FirebaseSettings
    {
        [JsonPropertyName("ProjectId")]
        public string ProjectId => "autismhelperdatabase-cec0f";

        [JsonPropertyName("PrivateKeyId")]
        public string PrivateKeyId => "AIzaSyA-1GVhqIZ2-uiqew4gKGlfqrbMz_D6Kz4";

    }
}

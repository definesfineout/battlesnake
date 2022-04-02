using Starter.Api.Requests;
using Starter.Api.Responses;
using System.Runtime.Serialization;

namespace Starter.Api
{
    [DataContract]
    public class MoveLog
    {
        [DataMember(Name="id")]
        public string Id { 
            get
            {
                if (Request?.Game == null)
                {
                    return null;
                }

                return $"{Request.Game.Id}_{Request.Turn}";
            }
        }

        [DataMember]
        public GameStatusRequest Request { get; set; }

        [DataMember]
        public MoveResponse Response { get; set; }
    }
}

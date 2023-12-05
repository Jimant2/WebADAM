using webapi.DefinitionModels;

namespace webapi.Models
{
    public class GroupChannelDTO
    {
        public string GroupName { get; set; }
        public List<ChannelReference> Channels { get; set; }
    }
}

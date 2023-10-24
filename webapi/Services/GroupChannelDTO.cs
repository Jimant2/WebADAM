using webapi.DefinitionModels;

namespace webapi.Services
{
    public class GroupChannelDTO
    {
        public string GroupName { get; set; }
        public ChannelReference[] Channels { get; set; }
    }
}

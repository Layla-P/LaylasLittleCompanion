using System.ComponentModel;

namespace LaylasLittleCompanion.Server.Models.Enums
{
    public enum ListEnum
    {
       
        Default =0,
        [Description("Todo")]
        Todo = 10,
        [Description("General Ideas")]
        General = 20,
        [Description("Bot Ideas")]
        Bot = 30,
        [Description("Links")]
        Links = 40
    }
}

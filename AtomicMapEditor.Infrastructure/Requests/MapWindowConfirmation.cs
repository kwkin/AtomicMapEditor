using Ame.Infrastructure.Models;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Infrastructure.Requests
{
    public class MapWindowConfirmation : Confirmation
    {
        public MapModel Map { get; set; }
    }
}

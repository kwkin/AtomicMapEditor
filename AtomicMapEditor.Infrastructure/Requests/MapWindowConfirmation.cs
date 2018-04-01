using AtomicMapEditor.Infrastructure.Models;
using Prism.Interactivity.InteractionRequest;

namespace AtomicMapEditor.Infrastructure.Requests
{
    public class MapWindowConfirmation : Confirmation
    {
        public MapModel Map { get; set; }
    }
}

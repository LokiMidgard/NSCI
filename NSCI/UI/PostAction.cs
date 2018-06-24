using System;
using System.Threading.Tasks;

namespace NSCI.UI
{
    internal struct PostAction
    {
        public TaskCompletionSource<bool> TaskSource { get; set; }
        public Action Action { get; set; }
    }
}
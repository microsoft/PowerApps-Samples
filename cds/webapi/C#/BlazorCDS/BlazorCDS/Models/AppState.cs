using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorCDS.Models
{
    public class AppState
    {
        public string SelectedEnvUrl { get; private set; }

        public event Action OnChange;

        public void SetEnvUrl(string envUrl)
        {
            SelectedEnvUrl = envUrl;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}

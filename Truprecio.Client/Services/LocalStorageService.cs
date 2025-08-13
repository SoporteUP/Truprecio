using Microsoft.JSInterop;

namespace Truprecio.Client.Services
{
    public class LocalStorageService
    {
        private readonly IJSRuntime _js;

        public LocalStorageService(IJSRuntime js)
        {
            _js = js;
        }

        public ValueTask SetItem(string key, string value) =>
            _js.InvokeVoidAsync("localStorage.setItem", key, value);

        public ValueTask<string> GetItem(string key) =>
            _js.InvokeAsync<string>("localStorage.getItem", key);

        public ValueTask RemoveItem(string key) =>
            _js.InvokeVoidAsync("localStorage.removeItem", key);
    }
}

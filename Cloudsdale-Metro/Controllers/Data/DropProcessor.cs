using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cloudsdale.Models.Json;
using Windows.UI.Core;

namespace Cloudsdale.Controllers.Data {
    public class DropProcessor {

        public DropProcessor(Cloud cloud) {
            _cloud = cloud;
        }

        private readonly ObservableCollection<Drop> _drops =
                     new ObservableCollection<Drop>();
        public ObservableCollection<Drop> Drops {
            get { return _drops; }
        }

        private readonly Cloud _cloud;
        private int _capacity = 1;

        public async Task<bool> AddMoreDrops() {
            ++_capacity;
            var result = await WebData.GetDataAsync<Drop[]>
                (WebserviceItem.Drops, _cloud.Id, null,
                 new Kvp("X-Result-Page", _capacity.ToString()));
            if (result.Data == null || result.Data.Length == 0) {
                return false;
            }
            var drops = result.Data;
            await Backload(drops);
            return true;
        }

        public async Task Backload(IEnumerable<Drop> drops) {
            if (Helpers.UIAccess) BackloadInternal(drops);
            else await Helpers.RunInUI(() => BackloadInternal(drops), CoreDispatcherPriority.Low);
        }
        private void BackloadInternal(IEnumerable<Drop> drops) {
            lock (_drops)
                foreach (var drop in drops) {
                    _drops.Add(drop);
                }
        }

        public async Task Add(Drop drop) {
            if (Helpers.UIAccess) AddInternal(drop);
            else await Helpers.RunInUI(() => AddInternal(drop), CoreDispatcherPriority.Low);
        }

        private void AddInternal(Drop drop) {
            lock (_drops) {
                var dtr = _drops.Where(d => d.Id == drop.Id).ToList();
                foreach (var d in dtr) {
                    _drops.Remove(d);
                }

                _drops.Insert(0, drop);
                while (_drops.Count > _capacity * 10) {
                    _drops.RemoveAt(_drops.Count - 1);
                }
            }
        }
    }
}

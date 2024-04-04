using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Extras {
    public class QualityOption {
        public string name;
        public int quality_index;

        public QualityOption(string name, int quality_index) {
            this.name = name;
            this.quality_index = quality_index;
        }
    }
}

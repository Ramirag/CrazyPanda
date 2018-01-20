using System;

namespace Core.Models
{
    public class Restaurant
    {
        private int _burgerCount;
        private bool _hotPieExist;
        private bool _restroomExist;


        public Restaurant(Point location, string name)
        {
            Location = location;
            Name = name;
        }

        public Point Location { get; }
        public string Name { get; }

        public bool RestroomExist
        {
            get => _restroomExist;
            set
            {
                if (value != _restroomExist)
                {
                    _restroomExist = value;
                    RestroomExistChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool HotPieExist
        {
            get => _hotPieExist;
            set
            {
                if (value != _hotPieExist)
                {
                    _hotPieExist = value;
                    HotPieExistChanged(this, EventArgs.Empty);
                }
            }
        }

        public int BurgerCount
        {
            get => _burgerCount;
            set
            {
                if (value != _burgerCount)
                {
                    _burgerCount = value;
                    BurgerCountChanged(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler RestroomExistChanged = (sender, args) => { };
        public event EventHandler HotPieExistChanged = (sender, args) => { };
        public event EventHandler BurgerCountChanged = (sender, args) => { };

        public override string ToString()
        {
            return $"{Name} Туалеты: {RestroomExist}, Пирожки: {HotPieExist}, Бургеры: {BurgerCount} шт.";
        }
    }
}
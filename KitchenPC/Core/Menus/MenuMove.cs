namespace KitchenPC.Menus
{
    using System;

    public class MenuMove
    {
        public Guid? TargetMenu { get; set; }

        public Guid[] RecipesToMove { get; set; }

        public bool MoveAll { get; set; }
    }
}
﻿using System;

using FFXIVClientStructs.FFXIV.Client.UI;

namespace ClickLib.Clicks
{
    /// <summary>
    /// Addon Request.
    /// </summary>
    public sealed unsafe class ClickRequest : ClickAddonBase<AddonRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClickRequest"/> class.
        /// </summary>
        /// <param name="addon">Addon pointer.</param>
        public ClickRequest(IntPtr addon = default)
            : base(addon)
        {
        }

        /// <inheritdoc/>
        protected override string AddonName => "Request";

        public static implicit operator ClickRequest(IntPtr addon) => new(addon);

        /// <summary>
        /// Instantiate this click using the given addon.
        /// </summary>
        /// <param name="addon">Addon to reference.</param>
        /// <returns>A click instance.</returns>
        public static ClickRequest Using(IntPtr addon) => new(addon);

        /// <summary>
        /// Click the hand over button.
        /// </summary>
        [ClickName("request_hand_over")]
        public void HandOver()
        {
            ClickAddonButton(&this.Addon->AtkUnitBase, this.Addon->HandOverButton, 0);
        }

        /// <summary>
        /// Click the cancel button.
        /// </summary>
        [ClickName("request_cancel")]
        public void Cancel()
        {
            ClickAddonButton(&this.Addon->AtkUnitBase, this.Addon->CancelButton, 1);
        }

        /// <summary>
        /// Right click item.
        /// </summary>
        [ClickName("request_item1")]
        public void Item1()
        {
            ClickAddonDragDrop(&this.Addon->AtkUnitBase, this.Addon->AtkComponentDragDrop250, 12);
        }
    }
}

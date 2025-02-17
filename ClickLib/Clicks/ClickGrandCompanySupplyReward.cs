﻿using System;

using FFXIVClientStructs.FFXIV.Client.UI;

namespace ClickLib.Clicks
{
    /// <summary>
    /// Addon GrandCompanySupplyReward.
    /// </summary>
    public sealed unsafe class ClickGrandCompanySupplyReward : ClickAddonBase<AddonGrandCompanySupplyReward>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClickGrandCompanySupplyReward"/> class.
        /// </summary>
        /// <param name="addon">Addon pointer.</param>
        public ClickGrandCompanySupplyReward(IntPtr addon = default)
            : base(addon)
        {
        }

        /// <inheritdoc/>
        protected override string AddonName => "GrandCompanySupplyReward";

        public static implicit operator ClickGrandCompanySupplyReward(IntPtr addon) => new(addon);

        /// <summary>
        /// Instantiate this click using the given addon.
        /// </summary>
        /// <param name="addon">Addon to reference.</param>
        /// <returns>A click instance.</returns>
        public static ClickGrandCompanySupplyReward Using(IntPtr addon) => new(addon);

        /// <summary>
        /// Click the deliver button.
        /// </summary>
        [ClickName("grand_company_expert_delivery_deliver")]
        public void Deliver()
        {
            ClickAddonButton(&this.Addon->AtkUnitBase, this.Addon->DeliverButton, 0);
        }

        /// <summary>
        /// Click the cancel button.
        /// </summary>
        [ClickName("grand_company_expert_delivery_cancel")]
        public void Cancel()
        {
            ClickAddonButton(&this.Addon->AtkUnitBase, this.Addon->CancelButton, 1);
        }
    }
}

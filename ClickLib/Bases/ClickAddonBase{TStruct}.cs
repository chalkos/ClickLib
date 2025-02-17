﻿using System;
using Dalamud.Hooking;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ClickLib
{
    /// <summary>
    /// AtkUnitBase receive event delegate.
    /// </summary>
    /// <param name="eventListener">Type receiving the event.</param>
    /// <param name="evt">Event type.</param>
    /// <param name="which">Internal routing number.</param>
    /// <param name="eventData">Event data.</param>
    /// <param name="inputData">Keyboard and mouse data.</param>
    /// <returns>The addon address.</returns>
    internal unsafe delegate IntPtr ReceiveEventDelegate(void* eventListener, EventType evt, uint which, IntPtr eventData, IntPtr inputData);

    /// <summary>
    /// Click base class.
    /// </summary>
    /// <typeparam name="TStruct">FFXIVClientStructs addon type.</typeparam>
    public abstract unsafe class ClickAddonBase<TStruct> : ClickBase where TStruct : unmanaged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClickAddonBase{TStruct}"/> class.
        /// </summary>
        /// <param name="addon">Addon address.</param>
        public ClickAddonBase(IntPtr addon)
        {
            if (addon == default)
                addon = this.GetAddonByName(this.AddonName);

            this.AddonAddress = addon;
            this.Addon = (TStruct*)addon;
        }

        /// <summary>
        /// Gets the associated addon name.
        /// </summary>
        protected abstract string AddonName { get; }

        /// <summary>
        /// Gets a pointer to the addon.
        /// </summary>
        protected IntPtr AddonAddress { get; }

        /// <summary>
        /// Gets a pointer to the type.
        /// </summary>
        protected TStruct* Addon { get; }

        /// <summary>
        /// Send a click.
        /// </summary>
        /// <param name="addonBase">The click recipient.</param>
        /// <param name="target">Target node.</param>
        /// <param name="which">Internal game click routing.</param>
        /// <param name="type">Event type.</param>
        protected static void ClickAddonButton(AtkUnitBase* addonBase, AtkComponentButton* target, uint which, EventType type = EventType.CHANGE)
            => ClickAddonComponent(addonBase, target->AtkComponentBase.OwnerNode, which, type);

        /// <summary>
        /// Send a click.
        /// </summary>
        /// <param name="addonBase">The click recipient.</param>
        /// <param name="target">Target node.</param>
        /// <param name="which">Internal game click routing.</param>
        /// <param name="type">Event type.</param>
        protected static void ClickAddonRadioButton(AtkUnitBase* addonBase, AtkComponentRadioButton* target, uint which, EventType type = EventType.CHANGE)
            => ClickAddonComponent(addonBase, target->AtkComponentBase.OwnerNode, which, type);

        /// <summary>
        /// Send a click.
        /// </summary>
        /// <param name="addonBase">The click recipient.</param>
        /// <param name="target">Target node.</param>
        /// <param name="which">Internal game click routing.</param>
        /// <param name="type">Event type.</param>
        protected static void ClickAddonCheckBox(AtkUnitBase* addonBase, AtkComponentCheckBox* target, uint which, EventType type = EventType.CHANGE)
            => ClickAddonComponent(addonBase, target->AtkComponentButton.AtkComponentBase.OwnerNode, which, type);

        /// <summary>
        /// Send a click.
        /// </summary>
        /// <param name="addonBase">The click recipient.</param>
        /// <param name="target">Target node.</param>
        /// <param name="which">Internal game click routing.</param>
        /// <param name="type">Event type.</param>
        protected static void ClickAddonDragDrop(AtkUnitBase* addonBase, AtkComponentDragDrop* target, uint which, EventType type = EventType.ICON_TEXT_ROLL_OUT)
            => ClickAddonComponent(addonBase, target->AtkComponentBase.OwnerNode, which, type);

        /// <summary>
        /// Send a click.
        /// </summary>
        /// <param name="addonBase">The click recipient.</param>
        /// <param name="target">Target node.</param>
        /// <param name="which">Internal game click routing.</param>
        /// <param name="type">Event type.</param>
        protected static void ClickAddonStage(AtkUnitBase* addonBase, AtkStage* target, uint which, EventType type = EventType.MOUSE_CLICK)
        {
            var eventData = EventData.ForNormalTarget(target, addonBase);
            var inputData = InputData.Empty();

            InvokeReceiveEvent(&addonBase->AtkEventListener, type, which, eventData, inputData);
        }

        /// <summary>
        /// Send a click.
        /// </summary>
        /// <param name="addonBase">The click recipient.</param>
        /// <param name="target">Target node.</param>
        /// <param name="which">Internal game click routing.</param>
        /// <param name="type">Event type.</param>
        /// <param name="eventData">Event data.</param>
        /// <param name="inputData">Input data.</param>
        protected static void ClickAddonComponent(AtkUnitBase* addonBase, AtkComponentNode* target, uint which, EventType type, EventData? eventData = null, InputData? inputData = null)
        {
            eventData ??= EventData.ForNormalTarget(target, addonBase);
            inputData ??= InputData.Empty();

            InvokeReceiveEvent(&addonBase->AtkEventListener, type, which, eventData, inputData);
        }

        /// <summary>
        /// Send a click.
        /// </summary>
        /// <param name="addonContextIconMenu">AddonContextIconMenu addon.</param>
        /// <param name="index">List index.</param>
        /// <param name="type">Event type.</param>
        protected static void ClickItemListRenderer(AddonContextIconMenu* addonContextIconMenu, ushort index)
        {
            var targetList = addonContextIconMenu->AtkComponentList240;
            if (index < 0 || index >= addonContextIconMenu->EntryCount)
                throw new ArgumentOutOfRangeException(nameof(index), "List index is out of range");

            var listItemRenderer = targetList->ItemRendererList[index].AtkComponentListItemRenderer;

            var eventData = EventData.ForNormalTarget(
                targetList->AtkComponentBase.OwnerNode,
                addonContextIconMenu);
            var inputData = InputData.ForContextIconMenu(&listItemRenderer->AtkComponentButton.AtkComponentBase.AtkEventListener, index);

            // if (firstEvent)
            // {
            //     InvokeReceiveEvent(
            //         &addonContextIconMenu->AtkUnitBase.AtkEventListener,
            //         EventType.LIST_ITEM_CLICK,
            //         3,
            //         eventData,
            //         inputData);
            // }
            // else
            // {
            //inputData.Data[2] = (void*)(index | ((ulong)index << 48));
                InvokeReceiveEvent(
                    &addonContextIconMenu->AtkUnitBase.AtkEventListener,
                    EventType.LIST_INDEX_CHANGE,
                    0,
                    eventData,
                    inputData);
            // }
        }

        private static void InvokeReceiveEvent2(AtkEventListener* eventListener, EventType type, uint which, EventData eventData,
            InputData inputData)
        {
            PluginLog.Debug($"{(ulong)eventListener:X},   {(ushort)type:X},   {which:X},   {(ulong)eventData.Data:X}->[0 {(ulong)eventData.Data[1]:X} {(ulong)eventData.Data[2]:X}],   {(ulong)inputData.Data}->[{(ulong)inputData.Data[0]:X}]");
        }

        /// <summary>
        /// Send a click.
        /// </summary>
        /// <param name="popupMenu">PopupMenu event listener.</param>
        /// <param name="index">List index.</param>
        /// <param name="type">Event type.</param>
        protected static void ClickAddonList(PopupMenu* popupMenu, ushort index, EventType type = EventType.LIST_INDEX_CHANGE)
        {
            var targetList = popupMenu->List;
            if (index < 0 || index >= popupMenu->EntryCount)
                throw new ArgumentOutOfRangeException(nameof(index), "List index is out of range");

            var eventData = EventData.ForNormalTarget(targetList->AtkComponentBase.OwnerNode, popupMenu);
            var inputData = InputData.ForPopupMenu(popupMenu, index);

            InvokeReceiveEvent(&popupMenu->AtkEventListener, type, 0, eventData, inputData);
        }

        private IntPtr GetAddonByName(string name, int index = 1)
        {
            var atkStage = AtkStage.GetSingleton();
            if (atkStage == null)
                throw new InvalidClickException("AtkStage is not available");

            var unitMgr = atkStage->RaptureAtkUnitManager;
            if (unitMgr == null)
                throw new InvalidClickException("UnitMgr is not available");

            var addon = unitMgr->GetAddonByName(name, index);
            if (addon == null)
                throw new InvalidClickException("Addon is not available");

            return (IntPtr)addon;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Rage;
using Rage.Native;

using RAGENativeUI.Elements;
using RAGENativeUI.Internals;

namespace RAGENativeUI
{
    public interface IScrollableListItem
    {
        public bool Selected { get; set; }

        public bool Skipped { get; set; }
    }

    public abstract class ScrollableListBase<T> where T : class, IScrollableListItem
    {
        protected abstract List<T> Items { get; set; }

        protected int maxItemsOnScreen = 15;
        protected int currentItem;
        protected int minItem;
        protected int maxItem;
        protected int hoveredItem = -1;

        /// <summary>
        /// Gets or set the maximum number of visible items.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the specified value is less than 1.</exception>

        public virtual int MaxItemsOnScreen
        {
            get => maxItemsOnScreen;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(MaxItemsOnScreen)} must be at least 1");
                }

                if (maxItemsOnScreen != value)
                {
                    maxItemsOnScreen = value;
                    UpdateVisibleItemsIndices();
                }
            }
        }

        /// <summary>
        /// Refreshes the current item index and min/max visible items, in case they are out of bounds.
        /// </summary>
        protected virtual void RefreshCurrentSelection()
        {
            CurrentSelection = GetIndexOfNextSelectableItem((CurrentSelection == -1 ? 0 : CurrentSelection) - 1);
        }


        /// <summary>
        /// Gets or sets the currently selected item index.
        /// A value of <c>-1</c> indicates that no selection exists, for example, when no items have been added to the menu.
        /// </summary>
        public virtual int CurrentSelection
        {
            get => currentItem;
            set
            {
                if (value != -1 && !IsValidItemIndex(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"Value must be -1 or in the range [0, {nameof(Items)}.{nameof(Items.Count)})");
                }

                if (Items.Count == 0)
                {
                    currentItem = -1;
                    minItem = -1;
                    maxItem = -1;
                }
                else
                {
                    bool isNewItem = currentItem != value || (IsValidItemIndex(value) && !Items[value].Selected);
                    if (isNewItem)
                    {
                        if (IsValidItemIndex(currentItem))
                        {
                            Items[currentItem].Selected = false;
                        }

                        currentItem = value;

                        if (IsValidItemIndex(currentItem))
                        {
                            Items[currentItem].Selected = true;
                        }
                    }

                    onSelectionChange(value, isNewItem);
                    UpdateVisibleItemsIndices();
                }
            }
        }

        public T CurrentItem
        {
            get
            {
                int index = CurrentSelection;
                if (IsValidItemIndex(index))
                {
                    return Items[index];
                }

                return null;
            }
        }

        protected virtual void onSelectionChange(int newItemIndex, bool isNewItem) { }

        /// <summary>
        /// Gets whether a selection exists. If the selection exists, <see cref="CurrentSelection"/> returns the index of the selected item; otherwise, <c>-1</c>.
        /// </summary>
        public bool HasSelection => IsValidItemIndex(CurrentSelection);

        /// <summary>
        /// Gets whether <paramref name="index"/> is in the range [0, Items.Count).
        /// </summary>
        protected bool IsValidItemIndex(int index) => index >= 0 && index < Items.Count;

        /// <summary>
        /// Gets the index of the first visible item.
        /// </summary>
        public int FirstItemOnScreen => minItem;

        /// <summary>
        /// Gets the index of the last visible item.
        /// </summary>
        public int LastItemOnScreen => maxItem;

        public IEnumerable<(int iterIndex, int itemIndex, T item, bool isItemSelected)> IterateVisibleItems()
        {
            for (int c = minItem; c <= maxItem; c++)
            {
                yield return (c - minItem, c, Items[c], c == CurrentSelection);
            }
        }

        /// <summary>
        /// Reset the current selected item to 0. Use this after you add or remove items from <see cref="Items"/> directly
        /// instead of through <see cref="AddItem(UIMenuItem)"/>, <see cref="AddItem(UIMenuItem, int)"/> or <see cref="RemoveItemAt(int)"/>.
        /// </summary>
        public virtual void RefreshIndex()
        {
            if (Items.Count == 0)
            {
                CurrentSelection = -1;
                return;
            }

            var selection = -1;
            for (int i = 0; i < Items.Count; i++)
            {
                if (selection == -1 && !Items[i].Skipped)
                {
                    selection = i;
                }
                Items[i].Selected = false;
            }

            CurrentSelection = selection;
        }

        /// <summary>
        /// Remove all items from the menu.
        /// </summary>
        public void Clear()
        {
            Items.Clear();
            CurrentSelection = -1;
        }


        public void MoveToPreviousItem()
        {
            CurrentSelection = GetIndexOfPreviousSelectableItem(CurrentSelection);
        }

        public void MoveToNextItem()
        {
            CurrentSelection = GetIndexOfNextSelectableItem(CurrentSelection);
        }

        protected int GetIndexOfPreviousSelectableItem(int startIndex)
            => GetIndexOfNextSelectableItem(startIndex, directionStep: -1);

        protected int GetIndexOfNextSelectableItem(int startIndex)
            => GetIndexOfNextSelectableItem(startIndex, directionStep: +1);

        protected int GetIndexOfNextSelectableItem(int startIndex, int directionStep)
        {
            int newSelection = startIndex;
            int count = 0; // keep count to prevent an infinite loop when all items are skipped
            do
            {
                newSelection = Common.Wrap(newSelection + directionStep, 0, Items.Count);
                count++;
            } while (count < Items.Count && Items[newSelection].Skipped);

            return Items[newSelection].Skipped ? -1 : newSelection;
        }

        protected void UpdateVisibleItemsIndices()
        {
            int maxItems = Math.Min(MaxItemsOnScreen, Items.Count);

            if (MaxItemsOnScreen >= Items.Count)
            {
                minItem = 0;
                maxItem = Items.Count - 1;
                return;
            }

            if (
                (currentItem == -1 || minItem == -1 || maxItem == -1) // if no selection or no previous selection
                || (maxItem < minItem) // if invalid range
                || (maxItem - minItem < maxItems - 1) // if not enough items
            ) 
            {
                minItem = 0;
                maxItem = maxItems - 1;
            }
            else if (currentItem < minItem) // moved selection up, out of current visible item
            {
                minItem = currentItem;
                maxItem = currentItem + maxItems - 1;
            }
            else if (currentItem > maxItem) // moved selection down, out of current visible item
            {
                minItem = currentItem - maxItems + 1;
                maxItem = currentItem;
            }
            else if (maxItem - minItem + 1 != MaxItemsOnScreen) // MaxItemsOnScreen changed
            {
                if (maxItem == currentItem)
                {
                    minItem = maxItem - maxItems + 1;
                }
                else
                {
                    maxItem = minItem + maxItems - 1;
                    if (maxItem >= Items.Count)
                    {
                        int diff = maxItem - Items.Count + 1;
                        maxItem -= diff;
                        minItem -= diff;
                    }
                }
            }
        }
    }
}

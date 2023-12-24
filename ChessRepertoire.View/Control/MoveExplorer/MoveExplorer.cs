using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ChessRepertoire.ViewModel.MoveExplorer;

namespace ChessRepertoire.View.Wpf.Control.MoveExplorer {
    internal class MoveExplorer : System.Windows.Controls.Control, IDisposable {
        private Panel? _itemsPanel;

        static MoveExplorer() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MoveExplorer), new FrameworkPropertyMetadata(typeof(MoveExplorer)));
        }


        #region Properties

        #region ItemsSource
        public IEnumerable<MoveViewModel>? ItemsSource {
            get => (IEnumerable<MoveViewModel>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public readonly static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<MoveViewModel>), typeof(MoveExplorer), new PropertyMetadata(null, OnItemsSourceChanged));
        #endregion

        #region ItemTemplate
        public DataTemplate ItemTemplate {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public readonly static DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(MoveExplorer), new PropertyMetadata(null));
        #endregion

        #endregion

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is not MoveExplorer control)
                return;

            if (e.OldValue is INotifyCollectionChanged oldCollection) {
                oldCollection.CollectionChanged -= control.OnCollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged newCollection) {
                newCollection.CollectionChanged += control.OnCollectionChanged;
            }

            control.Reset();
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            if (_itemsPanel == null)
                return;

            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    Console.WriteLine("Add");
                    break;

                case NotifyCollectionChangedAction.Remove:
                    Console.WriteLine("Remove");
                    break;

                case NotifyCollectionChangedAction.Replace:
                    Console.WriteLine("Replace");
                    break;

                case NotifyCollectionChangedAction.Move:
                    Console.WriteLine("Move");
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Console.WriteLine("Reset");
                    break;

                default:
                    throw new System.ArgumentOutOfRangeException(nameof(e.Action));
            }
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_ItemsPanel") is not Panel itemsPanel) return;
            _itemsPanel = itemsPanel;
        }


        private readonly ISet<INotifyCollectionChanged> _collectionChangedSubscriptions = new HashSet<INotifyCollectionChanged>();

        private void Reset() {
            _itemsPanel?.Children.Clear();
            ClearSubscriptions();

            if (ItemsSource != null)
                UpdateItems(ItemsSource);

        }

        private void ClearSubscriptions() {
            foreach (var collection in _collectionChangedSubscriptions) {
                collection.CollectionChanged -= OnCollectionChanged;
            }

            _collectionChangedSubscriptions.Clear();
        }

        private ContentPresenter CreateContentPresenter(object item) {
            return new ContentPresenter { Content = item, ContentTemplate = ItemTemplate };
        }

        private void UpdateItems(IEnumerable<MoveViewModel> itemsSource) {
            Debug.Assert(_itemsPanel != null && itemsSource != null);

            if (itemsSource is INotifyCollectionChanged collection) {
                collection.CollectionChanged += OnCollectionChanged;
                _collectionChangedSubscriptions.Add(collection);
            }

            foreach (var item in itemsSource) {
                var content = CreateContentPresenter(item);
                _itemsPanel.Children.Add(content);

                // We assume here that there are no loops in the tree. Proper handling of loops would require a set of visited nodes.
                // Also, we assume that the stack size is sufficient. If this assumption doesn't hold true we'd need to rewrite this to use
                // a queue instead.
                UpdateItems(item.Children.Items);
            }
        }

        public void Dispose() {
            ClearSubscriptions();
        }
    }
}

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace SageConnect.ViewModels
{
    class GridAnimation : AnimationTimeline
    {
        #region Override Properties

        /// <summary>
        /// Gets animated property type
        /// </summary>
        public override Type TargetPropertyType
        {
            get
            {
                return typeof(GridLength);
            }
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Overrided. Creates new Instance of animation.
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new GridAnimation();
        }

        /// <summary>
        /// Overrided. Returns new value of grid length each AnimationClock "tick" 
        /// </summary>
        /// <param name="defaultOriginValue"></param>
        /// <param name="defaultDestinationValue"></param>
        /// <param name="animationClock"></param>
        /// <returns></returns>
        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            double fromVal = ((GridLength)GetValue(FromProperty)).Value;
            double toVal = ((GridLength)GetValue(ToProperty)).Value;

            if (fromVal > toVal)
            {
                Debug.Assert(animationClock.CurrentProgress != null, "animationClock.CurrentProgress != null");
                return new GridLength((1 - animationClock.CurrentProgress.Value) * (fromVal - toVal) + toVal,
                    GridUnitType.Pixel);
            }
// ReSharper disable once RedundantIfElseBlock
            else
            {
                Debug.Assert(animationClock.CurrentProgress != null, "animationClock.CurrentProgress != null");
                return new GridLength(animationClock.CurrentProgress.Value * (toVal - fromVal) + fromVal,
                    GridUnitType.Pixel);
            }
        }

        #endregion

        #region Public Properties

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(GridLength),
                typeof(GridAnimation));

        /// <summary>
        /// Gets/sets start animation length
        /// </summary>
        public GridLength From
        {
            get
            {
                return (GridLength)GetValue(FromProperty);
            }
            set
            {
                SetValue(FromProperty, value);
            }
        }

        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(GridLength),
                typeof(GridAnimation));

        /// <summary>
        /// Gets/sets end animation length
        /// </summary>
        public GridLength To
        {
            get
            {
                return (GridLength)GetValue(ToProperty);
            }
            set
            {
                SetValue(ToProperty, value);
            }
        }

        #endregion


    }

    /// <summary>
    /// To call the Grid Animation
    /// </summary>
    public static class AnimateGrid
    {
        /// <summary>
        /// For animating grid colums
        /// </summary>
        /// <param name="gridColumn"></param>
        /// <param name="expand"></param>
        /// <param name="expandedWidth"></param>
        /// <param name="collapsedWidth"></param>
        /// <param name="minWidth"></param>
        /// <param name="seconds"></param>
        /// <param name="milliseconds"></param>
        public static void AnimateGridColumnExpandCollapse(ColumnDefinition gridColumn, bool expand, double expandedWidth, double collapsedWidth,
        double minWidth, int seconds, int milliseconds)
        {
            if (expand && gridColumn.ActualWidth >= expandedWidth)
                // It's as wide as it needs to be.
                return;

// ReSharper disable once CompareOfFloatsByEqualityOperator
            if (!expand && gridColumn.ActualWidth == collapsedWidth)
                // It's already collapsed.
                return;

            Storyboard storyBoard = new Storyboard();

            GridAnimation animation = new GridAnimation();
            animation.From = new GridLength(gridColumn.ActualWidth);
            animation.To = new GridLength(expand ? expandedWidth : collapsedWidth);
            animation.Duration = new TimeSpan(0, 0, 0, seconds, milliseconds);

            // Set delegate that will fire on completion.
            animation.Completed += delegate
            {
                // Set the animation to null on completion. This allows the grid to be resized manually
                gridColumn.BeginAnimation(ColumnDefinition.WidthProperty, null);

                // Set the final value manually.
                gridColumn.Width = new GridLength(expand ? expandedWidth : collapsedWidth);

                // Set the minimum width.
                gridColumn.MinWidth = minWidth;
            };

            storyBoard.Children.Add(animation);

            Storyboard.SetTarget(animation, gridColumn);
            Storyboard.SetTargetProperty(animation, new PropertyPath(ColumnDefinition.WidthProperty));
            storyBoard.Children.Add(animation);

            // Begin the animation.
            storyBoard.Begin();
        }
        /// <summary>
        /// To Identify the login form for animation
        /// </summary>
        public static ColumnDefinition LoginForm;
        /// <summary>
        /// To Identify the connection form for animation
        /// </summary>
        public static ColumnDefinition ConnectionForm;

        /// <summary>
        /// To Identify the details form for animation
        /// </summary>
        public static ColumnDefinition DetailColumn ;

        /// <summary>
        /// To Identify the connections column for animation
        /// </summary>
        public static ColumnDefinition ConnectionsColumn;


        /// <summary>
        /// To Expand ConnectionDetails
        /// </summary>
        public static void ExpandConnectionDetails(bool singletenant=true)
        {
            int expandWidth = 450;
            int reducedwidth = 350;
            //if (singletenant)
            //{
            //    expandWidth = 550;
            //    reducedwidth = 250;
            //}

            AnimateGridColumnExpandCollapse(ConnectionsColumn, false, 0, reducedwidth, 0, 0, 250);
            AnimateGridColumnExpandCollapse(DetailColumn, true, expandWidth, 0, 0, 0, 250);
            DecreaseControlswidth(false);
        }

        /// <summary>
        /// To collapse the ConnectionDetail 
        /// </summary>
        public static void CollapseConnectionDetails()
        {
            AnimateGridColumnExpandCollapse(DetailColumn, false, 0, 0, 0, 0, 250);
            AnimateGridColumnExpandCollapse(ConnectionsColumn, true, 740, 0, 0, 0, 250);
            DecreaseControlswidth(true);
        }

        /// <summary>
        /// To Expand Login form
        /// </summary>
        public static void ExpandLoginForm()
        {
            AnimateGridColumnExpandCollapse(ConnectionForm, false, 0, 0, 0, 0, 250);
            AnimateGridColumnExpandCollapse(LoginForm, true, 750, 0, 0, 0, 250);
        }

        /// <summary>
        /// To collapse the Login Form
        /// </summary>
        public static void CollapseLoginForm()
        {
            AnimateGridColumnExpandCollapse(LoginForm, false, 0, 0, 0, 0, 250);
            AnimateGridColumnExpandCollapse(ConnectionForm, true, 750, 0, 0, 0, 250);
        }

        private static void DecreaseControlswidth(bool increase)
        {
            double width = ConnectionStatusDataGrid.Width;
            if ((width <= 335 && !increase) || (width >= 716 && increase))
                return;
            ConnectionMessageBlock.HorizontalAlignment = HorizontalAlignment.Left;
            ConnectionStatusDataGrid.HorizontalAlignment=HorizontalAlignment.Left;
            if (!increase)
            {
                ConnectionStatusDataGrid.Width =335;
                ConnectionMessageBlock.Width =  330;
                foreach (var column in ConnectionStatusDataGrid.Columns)
                {
                    if (column.DisplayIndex == 0)
                        continue;
                    if (column.DisplayIndex == 1)
                    {
                        column.Width = column.Width.Value/(ConnectionStatusDataGrid.VerticalScrollBarVisibility == ScrollBarVisibility.Visible?2.46:2.34);
                        continue;
                    }
                    column.Width = column.Width.Value/2.1;
                }
                return;
            }
            ConnectionStatusDataGrid.Width = 716;
            ConnectionMessageBlock.Width = 600;
            foreach (var column in ConnectionStatusDataGrid.Columns)
            {
                if (column.DisplayIndex == 0)
                    continue;

                if (column.DisplayIndex == 1)
                {
                    column.Width = column.Width.Value * (ConnectionStatusDataGrid.VerticalScrollBarVisibility == ScrollBarVisibility.Visible ? 2.46 : 2.34); 
                    continue;
                }
                column.Width = column.Width.Value * 2.1;
            }

            //double width = ConnectionStatusDataGrid.Width;
            //if ((width < 400 && !increase) || (width >= 716 && increase))  
            //    return;
            //ConnectionStatusDataGrid.HorizontalAlignment=HorizontalAlignment.Left;
            //width = 0;
            //foreach (var column in ConnectionStatusDataGrid.Columns)
            //{
            //    if (Math.Abs(width) > 0)
            //    {
            //        column.Width = ((increase ? column.Width.Value*2.11 : column.Width.Value/2.11)); // +
            //        //(ConnectionStatusDataGrid.Items.Count > 4 && !increase? -5: 5));
            //    }
            //    width += column.Width.Value ;
            //}

            //ConnectionStatusDataGrid.Width = (increase ? 716 : width );
            //ConnectionMessageBlock.Width = (increase ? ConnectionMessageBlock.Width * 2 : 300);
            //ConnectionMessageBlock.HorizontalAlignment = HorizontalAlignment.Left;


        }

        /// <summary>
        /// 
        /// </summary>
        public static DataGrid ConnectionStatusDataGrid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static TextBlock ConnectionMessageBlock { get; set; }
    }
}

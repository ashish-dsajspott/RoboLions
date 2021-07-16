﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rubyer
{
    public class MessageCard : ContentControl
    {
        static MessageCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageCard), new FrameworkPropertyMetadata(typeof(MessageCard)));
        }

        public static readonly DependencyProperty CornerRadiusProperty =
          DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MessageCard), new PropertyMetadata(default(CornerRadius)));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }


        public static readonly DependencyProperty ThemeColorBrushProperty =
            DependencyProperty.Register("ThemeColorBrush", typeof(SolidColorBrush), typeof(MessageCard), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush ThemeColorBrush
        {
            get { return (SolidColorBrush)GetValue(ThemeColorBrushProperty); }
            set { SetValue(ThemeColorBrushProperty, value); }
        }


        public static readonly DependencyProperty IconTypeProperty =
            DependencyProperty.Register("IconType", typeof(IconType), typeof(MessageCard), new PropertyMetadata(default(IconType)));

        public IconType IconType
        {
            get { return (IconType)GetValue(IconTypeProperty); }
            set { SetValue(IconTypeProperty, value); }
        }


        public static readonly DependencyProperty IsClearableProperty =
            DependencyProperty.Register("IsClearable", typeof(bool), typeof(MessageCard), new PropertyMetadata(default(bool), OnIsClearbleChanged));

        public bool IsClearable
        {
            get { return (bool)GetValue(IsClearableProperty); }
            set { SetValue(IsClearableProperty, value); }
        }

        private static void OnIsClearbleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MessageCard messageCard)
            {
                RoutedEventHandler handle = (sender, args) =>
                {
                    if (VisualTreeHelper.GetParent(messageCard) is Panel panel)
                    {
                        // 退出动画
                        Storyboard exitStoryboard = new Storyboard();

                        DoubleAnimation exitOpacityAnimation = new DoubleAnimation
                        {
                            From = 1,
                            To = 0,
                            Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
                        };
                        Storyboard.SetTargetProperty(exitOpacityAnimation, new PropertyPath(FrameworkElement.OpacityProperty));

                        DoubleAnimation exitTransformAnimation = new DoubleAnimation
                        {
                            From = 0,
                            To = -30,
                            Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
                        };
                        Storyboard.SetTargetProperty(exitTransformAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

                        exitStoryboard.Children.Add(exitOpacityAnimation);
                        exitStoryboard.Children.Add(exitTransformAnimation);

                        // 动画完成
                        exitStoryboard.Completed += (a, b) =>
                        {
                            panel.Children.Remove(messageCard);
                        };

                        messageCard.BeginStoryboard(exitStoryboard);    // 执行动画
                    }
                };

                messageCard.Loaded += (sender, arg) =>
                {
                    if (messageCard.Template.FindName("clearButton", messageCard) is Button clearButton)
                    {
                        if (messageCard.IsClearable)
                        {
                            clearButton.Click += handle;
                        }
                        else
                        {
                            clearButton.Click -= handle;
                        }
                    }
                };

                messageCard.Unloaded += (sender, arg) =>
                {
                    if (messageCard.Template.FindName("clearButton", messageCard) is Button clearButton)
                    {
                        if (messageCard.IsClearable)
                        {
                            clearButton.Click -= handle;
                        }
                    }
                };
            }
        }
    }
}
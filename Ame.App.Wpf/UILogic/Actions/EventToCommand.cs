﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Ame.App.Wpf.UILogic.Actions
{
    public class EventToCommand : TriggerAction<DependencyObject>
    {
        #region properties

        public static readonly DependencyProperty EventNameProperty =
            DependencyProperty.Register(
                nameof(EventName),
                typeof(string),
                typeof(EventToCommand),
                new PropertyMetadata(default(string)));

        public string EventName
        {
            get { return (string)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(EventToCommand),
                new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                nameof(CommandParameter),
                typeof(object),
                typeof(EventToCommand),
                new PropertyMetadata(default(object)));

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty EventArgsConverterProperty =
            DependencyProperty.Register(
                nameof(EventArgsConverter),
                typeof(IValueConverter),
                typeof(EventToCommand),
                new PropertyMetadata(default(IValueConverter)));

        public IValueConverter EventArgsConverter
        {
            get { return (IValueConverter)GetValue(EventArgsConverterProperty); }
            set { SetValue(EventArgsConverterProperty, value); }
        }

        public static readonly DependencyProperty EventArgsConverterParameterProperty =
            DependencyProperty.Register(
                nameof(EventArgsConverterParameter),
                typeof(object),
                typeof(EventToCommand),
                new PropertyMetadata(default(object)));

        public object EventArgsConverterParameter
        {
            get { return GetValue(EventArgsConverterParameterProperty); }
            set { SetValue(EventArgsConverterParameterProperty, value); }
        }

        public static readonly DependencyProperty EventArgsParameterPathProperty =
            DependencyProperty.Register(
                nameof(EventArgsParameterPath),
                typeof(string),
                typeof(EventToCommand),
                new PropertyMetadata(default(string)));

        public string EventArgsParameterPath
        {
            get { return (string)GetValue(EventArgsParameterPathProperty); }
            set { SetValue(EventArgsParameterPathProperty, value); }
        }

        private object commandParameterValue;
        public object CommandParameterValue
        {
            get { return commandParameterValue ?? CommandParameter; }
            set { commandParameterValue = value; }
        }

        public bool PassEventArgsToCommand { get; set; }

        #endregion properties


        #region methods

        private ICommand GetCommand()
        {
            return Command;
        }

        protected override void Invoke(object parameter)
        {
            var command = GetCommand();
            var commandParameter = CommandParameterValue;

            if (commandParameter == null)
            {
                commandParameter = EventArgsConverter == null
                    ? parameter
                    : EventArgsConverter.Convert(parameter, typeof(object), EventArgsConverterParameter, CultureInfo.CurrentUICulture);
            }

            if (command != null && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }

        protected virtual void OnEventRaised(object sender, EventArgs eventArgs)
        {
            if (Command == null)
            {
                return;
            }

            var parameter = CommandParameter;

            if (parameter == null && !string.IsNullOrEmpty(EventArgsParameterPath))
            {
                //Walk the ParameterPath for nested properties.
                var propertyPathParts = EventArgsParameterPath.Split('.');
                object propertyValue = eventArgs;
                foreach (var propertyPathPart in propertyPathParts)
                {
                    var propInfo = propertyValue.GetType().GetTypeInfo().GetDeclaredProperty(propertyPathPart);
                    propertyValue = propInfo.GetValue(propertyValue);
                    if (propertyValue == null)
                    {
                        break;
                    }
                }
                parameter = propertyValue;
            }

            if (parameter == null && eventArgs != null && eventArgs != EventArgs.Empty && EventArgsConverter != null)
            {
                parameter = EventArgsConverter.Convert(eventArgs, typeof(object), EventArgsConverterParameter,
                    CultureInfo.CurrentUICulture);
            }

            if (Command.CanExecute(parameter))
            {
                Command.Execute(parameter);
            }
        }

        #endregion methods
    }
}

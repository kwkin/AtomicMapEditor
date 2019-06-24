using Ame.Infrastructure.Core;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using AvalonDock;
using ICSharpCode.AvalonEdit.Utils;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Ame.App.Wpf.UI.Serializer
{
    public class DockLayoutViewModel
    {
        #region fields

        private ILayoutViewModel layoutParent = null;
        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructor

        public DockLayoutViewModel(ILayoutViewModel parent, IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.layoutParent = parent;
            this.eventAggregator = eventAggregator;
        }

        #endregion constructor


        #region properties

        public string CurrentLayout { get; private set; }

        private DelegateCommand saveLayoutToStringCommand = null;
        public ICommand SaveLayoutToStringCommand
        {
            get
            {
                if (this.saveLayoutToStringCommand == null)
                {
                    this.saveLayoutToStringCommand = new DelegateCommand(
                        this.SaveLayoutToString,
                        () => !this.layoutParent.IsBusy.Value);
                }
                return this.saveLayoutToStringCommand;
            }
        }

        private DelegateCommand loadLayoutFromStringCommand = null;
        public ICommand LoadLayoutFromStringCommand
        {
            get
            {
                if (this.loadLayoutFromStringCommand == null)
                {
                    this.loadLayoutFromStringCommand = new DelegateCommand(
                        this.LoadLayoutFromString,
                        () => !this.layoutParent.IsBusy.Value && !string.IsNullOrEmpty(this.CurrentLayout));
                }
                return this.loadLayoutFromStringCommand;
            }
        }

        private DelegateCommand<object> loadLayoutCommand = null;
        public ICommand LoadLayoutCommand
        {
            get
            {
                if (this.loadLayoutCommand == null)
                {
                    this.loadLayoutCommand = new DelegateCommand<object>((p) =>
                    {
                        DockingManager docManager = p as DockingManager;
                        if (docManager == null)
                        {
                            return;
                        }

                        try
                        {
                            this.LoadLayoutFromFile();
                        }
                        catch
                        {
                        }
                        this.layoutParent.IsBusy.Value = false;
                    });
                }
                return this.loadLayoutCommand;
            }
        }

        private DelegateCommand<object> saveLayoutCommand = null;
        public ICommand SaveLayoutCommand
        {
            get
            {
                if (this.saveLayoutCommand == null)
                {
                    this.saveLayoutCommand = new DelegateCommand<object>((p) =>
                    {
                        string xmlLayout = p as string;
                        if (xmlLayout == null)
                        {
                            return;
                        }

                        this.SaveLayoutToFile(xmlLayout);
                    });
                }

                return this.saveLayoutCommand;
            }
        }

        #endregion properties


        #region methods

        #region LoadLayout

        private void LoadLayoutFromString()
        {
            if (string.IsNullOrEmpty(this.CurrentLayout) == true)
            {
                return;
            }

            NotificationMessage<string> notification = new NotificationMessage<string>(
                this.CurrentLayout,
                MessageIds.LoadWorkspaceLayout);
            this.eventAggregator.GetEvent<NotificationEvent<string>>().Publish(notification);
        }

        private void LoadLayoutFromFile()
        {
            try
            {
                this.layoutParent.IsBusy.Value = true;
                string layoutFile = Global.LayoutFileName;
                if (!File.Exists(layoutFile))
                {
                    throw new FileNotFoundException("Layout file not found");
                }

                Task taskToProcess = Task.Factory.StartNew<string>((stateObj) =>
                {
                    string xml = string.Empty;
                    try
                    {
                        using (FileStream fs = new FileStream(layoutFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using (StreamReader reader = FileReader.OpenStream(fs, Encoding.Default))
                            {
                                xml = reader.ReadToEnd();
                            }
                        }
                    }
                    catch (OperationCanceledException operationCanceledExp)
                    {
                        throw operationCanceledExp;
                    }
                    catch (Exception exp)
                    {
                        throw exp;
                    }
                    return xml;
                }, null).ContinueWith(ant =>
                {
                    try
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            NotificationMessage<string> notification = new NotificationMessage<string>(ant.Result, MessageIds.LoadWorkspaceLayout);
                            this.eventAggregator.GetEvent<NotificationEvent<string>>().Publish(notification);
                        }),
                        DispatcherPriority.Background);
                    }
                    catch (AggregateException aggExp)
                    {
                        throw new Exception("Error loading layout.", aggExp);
                    }
                });
            }
            catch (FileNotFoundException fileNotFoundExp)
            {
                throw fileNotFoundExp;
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this.layoutParent.IsBusy.Value = false;
            }
        }

        #endregion LoadLayout

        #region SaveLayout

        private void SaveLayoutToString()
        {
            NotificationActionMessage<string> notification = new NotificationActionMessage<string>(
                MessageIds.SaveWorkspaceLayout,
                (result) =>
                {
                    this.layoutParent.IsBusy.Value = true;
                    CommandManager.InvalidateRequerySuggested();
                    this.CurrentLayout = result;
                    this.layoutParent.IsBusy.Value = false;
                });
            this.eventAggregator.GetEvent<NotificationActionEvent<string>>().Publish(notification);
        }

        private void SaveLayoutToFile(string xmlLayout)
        {
            if (xmlLayout == null)
            {
                return;
            }

            File.WriteAllText(Global.LayoutFileName, xmlLayout);
        }

        #endregion SaveLayout

        #endregion methods
    }
}

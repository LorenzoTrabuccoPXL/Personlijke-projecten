using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.IO;


namespace Todo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Todos> _todos = new List<Todos>();
        string _bestandPad = "Todos.json";
        Todos _todo = new Todos();

        public MainWindow()
        {
            InitializeComponent();
            deadlineDatepicker.DisplayDateStart = DateTime.Today;
            omschrijvingChangeStackPanel.Visibility = Visibility.Collapsed;
            // Zet het opgeslagen bestand om naar een list van todos 
            string json = File.ReadAllText(_bestandPad);
            _todos = JsonSerializer.Deserialize<List<Todos>>(json) ?? new List<Todos>();
            AssignTodos();
        }

        private void addTodoButton_Click(object sender, RoutedEventArgs e)
        {
            //alle inputs moeeten ingevuld zijn voor een todo kan toegevoegd worden
            if (!string.IsNullOrWhiteSpace(nameTextBox.Text) 
                && statusComboBox.SelectedIndex != -1 
                && deadlineDatepicker.SelectedDate > DateTime.Today 
                && !string.IsNullOrWhiteSpace(omschrijvingTextBox.Text))
            {
                _todo.name = nameTextBox.Text;
                _todo.omschrijving = omschrijvingTextBox.Text;
                _todo.deadlineDate = (DateTime)deadlineDatepicker.SelectedDate;

                var selectedSatus = (ComboBoxItem)statusComboBox.SelectedItem;
                string status = selectedSatus.Content.ToString();
                _todo.status = status;

                _todos.Add(_todo);
                AssignTodos();
            }
        }

        private void AssignTodos()
        {
            SaveList();
            ClearListBoxes();
            //Vult de listboxes
            foreach (Todos todo in _todos)
            {
                switch (todo.status.ToString())
                {
                    case "todo":
                        todoListBox.Items.Add(todo);
                        break;
                    case "doing":
                        doingListBox.Items.Add(todo);
                        break;
                    case "done":
                        doneListBox.Items.Add(todo);
                        break;
                }
            } 
        }

        private void SaveList()
        {
            string jsonOpslaan = JsonSerializer.Serialize(_todos, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(_bestandPad, jsonOpslaan);
        }

        private void ClearListBoxes()
        {
            todoListBox.Items.Clear();
            doingListBox.Items.Clear();
            doneListBox.Items.Clear();
        }
       
        private void ShowInfo(ListBox sender)
        {
            if (sender.SelectedItem == null)
                return;

            _todo = (Todos)sender.SelectedItem;
            infoTextBlock.Text = $"deadline: {_todo.deadlineDate}\n\n{_todo.omschrijving}";
        }

        private void AddToListBox(Todos todo, string status)
        {
            todo.status = status;
            SaveList();
            AssignTodos();
        }

        private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //deselecteerd de andere listboxitems
            if (sender == todoListBox)
            {
                doingListBox.SelectedItem = null;
                doneListBox.SelectedItem = null;
            }
            else if (sender == doneListBox)
            {
                doingListBox.SelectedItem = null;
                todoListBox.SelectedItem = null;
            }
            else
            {
                todoListBox.SelectedItem = null;
                doneListBox.SelectedItem = null;
            }
            ShowInfo((ListBox)sender);
        }


        private void AddToButton_Click(object sender, RoutedEventArgs e)
        {
            //Als op de sender geklikt wordt status van de todo gewijzigd en in de juiste listbox gezet
            if (sender == todoAddButton)
            {
                if (doingListBox.SelectedItem != null)
                {
                    AddToListBox((Todos)doingListBox.SelectedItem, "todo");
                }
                else if (doneListBox.SelectedItem != null)
                {
                    AddToListBox((Todos)doneListBox.SelectedItem, "todo");
                }
            }
            else if (sender == doingAddButton)
            {
                if (todoListBox.SelectedItem != null)
                {
                    AddToListBox((Todos)todoListBox.SelectedItem, "doing");
                }
                else if (doneListBox.SelectedItem != null)
                {
                    AddToListBox((Todos)doneListBox.SelectedItem, "doing");
                }
            }
            else
            {
                if (todoListBox.SelectedItem != null)
                {
                    AddToListBox((Todos)todoListBox.SelectedItem, "done");
                }
                else if (doingListBox.SelectedItem != null)
                {
                    AddToListBox((Todos)doingListBox.SelectedItem, "done");
                }
            }
        }

        private void DeleteTodo_Click(object sender, RoutedEventArgs e)
        {
            //Verwijdert het item uit de geselecteerde listbox
            if(todoListBox.SelectedItem != null)
            {
                _todos.Remove((Todos)todoListBox.SelectedItem);
            }
            else if(doingListBox.SelectedItem != null)
            {
                _todos.Remove((Todos)doingListBox.SelectedItem);
            }
            else if(doneListBox.SelectedItem != null)
            {
                _todos.Remove((Todos)doneListBox.SelectedItem);
            }
            AssignTodos();
        }

        private void Omschrijving_Click(object sender, RoutedEventArgs e)
        {
            if (todoListBox.SelectedItem != null || doingListBox.SelectedItem != null || doneListBox.SelectedItem != null)
            {
                omschrijvingChangeStackPanel.Visibility = Visibility.Visible;
                omschrijvingChangeTextBox.Text = "";
            }
        }

        private void cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            omschrijvingChangeStackPanel.Visibility = Visibility.Collapsed;
        }

        private void ChangeOmschrijvingTodo(Todos todo)
        {
            todo.omschrijving = omschrijvingChangeTextBox.Text;
            AssignTodos();
        }

        private void changebutton_Click(object sender, RoutedEventArgs e)
        {
            if (todoListBox.SelectedItem != null)
            {
                ChangeOmschrijvingTodo((Todos)todoListBox.SelectedItem);
            }
            else if (doingListBox.SelectedItem != null)
            {
                ChangeOmschrijvingTodo((Todos)doingListBox.SelectedItem);
            }
            else if (doneListBox.SelectedItem != null)
            {
                ChangeOmschrijvingTodo((Todos)doneListBox.SelectedItem);
            }
            omschrijvingChangeStackPanel.Visibility = Visibility.Collapsed;
        }
    }
}
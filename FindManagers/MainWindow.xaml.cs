using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NewEmployeeDLL;
using NewEventLogDLL;

namespace FindManagers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        FindAllActiveEmployeeInformationDataSet TheFindAllActiveEmployeeInformationDateSet = new FindAllActiveEmployeeInformationDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        BlueJayManagersDataSet TheBlueJayManagerDataSet = new BlueJayManagersDataSet();
        FindEmployeeIsAManagerDataSet TheFindEmployeeIsAManagerDataSet = new FindEmployeeIsAManagerDataSet();

        //setting up variables
        int gintCounter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetWindow();
        }
        private void ResetWindow()
        {
            TheBlueJayManagerDataSet.bluejaymanagers.Rows.Clear();

            TheFindAllActiveEmployeeInformationDateSet = TheEmployeeClass.FindAllActiveEmployeeInformation();

            dgrManagers.ItemsSource = TheBlueJayManagerDataSet.bluejaymanagers;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnLoadGrid_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intManagerID;
            int intSecondCounter;
            string strFirstName;
            string strLastName;
            string strEmailAddress;
            string strDepartment;
            bool blnItemFound = false;
            string strEmployeeGroup = "";

            try
            {
                TheBlueJayManagerDataSet.bluejaymanagers.Rows.Clear();
                gintCounter = 0;

                intNumberOfRecords = TheFindAllActiveEmployeeInformationDateSet.FindAllActiveEmployeeInformation.Rows.Count;

                if(intNumberOfRecords < 1)
                {
                    TheMessagesClass.ErrorMessage("Data Set Not Loaded, Stopping Method");

                    return;
                }

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    blnItemFound = false;

                    intManagerID = TheFindAllActiveEmployeeInformationDateSet.FindAllActiveEmployeeInformation[intCounter].ManagerID;
                    strEmployeeGroup = TheFindAllActiveEmployeeInformationDateSet.FindAllActiveEmployeeInformation[intCounter].EmployeeGroup;

                    if(strEmployeeGroup == "MANAGERS")
                    {
                        intManagerID = TheFindAllActiveEmployeeInformationDateSet.FindAllActiveEmployeeInformation[intCounter].ManagerID;
                    }

                    
                    
                    
                    if(gintCounter > 0)
                    {
                        for(intSecondCounter = 0; intSecondCounter < gintCounter; intSecondCounter++)
                        {
                            if(intManagerID == TheBlueJayManagerDataSet.bluejaymanagers[intSecondCounter].EmployeeID)
                            {
                                blnItemFound = true;
                            }
                        }
                    }

                    if(blnItemFound == false)
                    {
                        TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intManagerID);

                        if(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].Active == true)
                        {
                            strFirstName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                            strLastName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;
                            strEmailAddress = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;
                            strDepartment = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].Department;

                            BlueJayManagersDataSet.bluejaymanagersRow NewManagerRow = TheBlueJayManagerDataSet.bluejaymanagers.NewbluejaymanagersRow();

                            NewManagerRow.Department = strDepartment;
                            NewManagerRow.EmailAddress = strEmailAddress;
                            NewManagerRow.EmployeeID = intManagerID;
                            NewManagerRow.FirstName = strFirstName;
                            NewManagerRow.LastName = strLastName;

                            TheBlueJayManagerDataSet.bluejaymanagers.Rows.Add(NewManagerRow);
                            gintCounter++;
                        }

                        
                    }
                }

                dgrManagers.ItemsSource = TheBlueJayManagerDataSet.bluejaymanagers;
            }
            catch (Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Find Managers // Main Window // Find Managers Button " + ex.Message);

                TheMessagesClass.ErrorMessage(ex.ToString());
            }
        }

        private void btnUpLoadInfo_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intManagerID;
            bool blnFatalError;

            try
            {
                intNumberOfRecords = TheBlueJayManagerDataSet.bluejaymanagers.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intManagerID = TheBlueJayManagerDataSet.bluejaymanagers[intCounter].EmployeeID;

                        TheFindEmployeeIsAManagerDataSet = TheEmployeeClass.FindEmployeeIsAManager(intManagerID);

                        if(TheFindEmployeeIsAManagerDataSet.FindEmployeeIsAManager[0].IsManager == false)
                        {
                           blnFatalError = TheEmployeeClass.UpdateManagerPosition(intManagerID, true);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                TheMessagesClass.InformationMessage("Managers Have Been Updated");

                ResetWindow();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Find Managers // Main Window // Upload Info Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}

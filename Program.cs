using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace bankConsoleDemo
{

    class Program
    {

        static void Main(string[] args)
        {
            //System.Data.Datatable as account database; create and populate it 
            DataTable dtAccount = new DataTable(); 
            cAccount.accountStarting(ref dtAccount);

            //System.Data.Datatable as transaction database ; create and populate it 
            DataTable dtTransaction = new DataTable();
            cTransaction.transactionStarting(ref dtTransaction);

            //test
            cTest.deposit(ref dtAccount, ref dtTransaction);
            cTest.withdraw(ref dtAccount, ref dtTransaction);
            cTest.transfer(ref dtAccount, ref dtTransaction);

            //print
            Console.WriteLine("Accounts");
            cAccount.accountPrinting(ref dtAccount);
            Console.WriteLine("========================");
            Console.WriteLine("Transactions");
            cTransaction.transactionPrinting(ref dtTransaction);

            //wait for user 
            Console.ReadKey();

        }
    }//end Program


    class cTest
    {

        static internal void deposit(ref DataTable dtAccount, ref DataTable dtTransaction)
        {
            cTransaction.transactionExecute(ref dtAccount, ref dtTransaction, "Deposit", 600, 2);
        }

        static internal void withdraw(ref DataTable dtAccount, ref DataTable dtTransaction)
        {
            cTransaction.transactionExecute(ref dtAccount, ref dtTransaction, "Withdrawal", 555, 2);
        }

        static internal void transfer(ref DataTable dtAccount, ref DataTable dtTransaction)
        {
            cTransaction.transactionExecute(ref dtAccount, ref dtTransaction, "Withdrawal", 300, 1, "Transfer");
            cTransaction.transactionExecute(ref dtAccount, ref dtTransaction, "Deposit", 300, 2, "Transfer");
        }

    }//end of cTest=======================================================================================


    //transaction=======================================================================
    class cTransaction
    {

        static internal void transactionPrinting(ref DataTable dtTransaction)
        {
            Console.WriteLine("{0,15}{1,15}{2,15}{3,15}",
                 dtTransaction.Columns[0].ColumnName,
                 dtTransaction.Columns[1].ColumnName,
                 dtTransaction.Columns[2].ColumnName,
                 dtTransaction.Columns[3].ColumnName
                 );

            foreach (DataRow dr in dtTransaction.Rows)
            {
                Console.WriteLine("{0,15}{1,15}{2,15}{3,15}",
                                   dr[0],
                                   "$" + dr[1].ToString(),
                                   dr[2].ToString(),
                                   dr[3].ToString()
                                   );
            }
        }

        
        static internal void transactionExecute(ref DataTable dtAccount, ref DataTable dtTransaction, string sTransactionType, decimal fTransactionAmount, int iAccountID, string sNote = "")
        {

            //get ref to account datarow
            DataRow drAccount = null;
            foreach (DataRow dr1 in dtAccount.Rows)
            {
                if (iAccountID == (int)dr1["iAccountID"])
                {
                    drAccount = dr1;
                    break;
                }
            }

            //check limit
            if ( ! cAccount.accountLimit( ref drAccount, ref sTransactionType, ref fTransactionAmount ))  return;
            
            //add transaction
            DataRow dr = dtTransaction.NewRow();
            dr[0] = sTransactionType; dr[1] = fTransactionAmount; dr[2] = iAccountID; dr[3] = sNote;
            dtTransaction.Rows.Add(dr);
            dtTransaction.AcceptChanges();

            //make negative
            if (sTransactionType == "Withdrawal") fTransactionAmount = fTransactionAmount * -1;
            
            //adjust accout
            drAccount["fBalance"] = (decimal)drAccount["fBalance"] + fTransactionAmount;
            dtAccount.AcceptChanges();

        }

        
        static internal void transactionStarting(ref DataTable dt)
        {
            dt = new DataTable();
            dt.Columns.Add("sTransactionType", typeof(System.String));
            dt.Columns.Add("fAmount", typeof(System.Decimal));
            dt.Columns.Add("iAccountID", typeof(System.Int64));
            dt.Columns.Add("sNote", typeof(System.String));
            dt.AcceptChanges();
        }
 
    }//end cTransaction=====================================================================



    //account=======================================================================
    class cAccount 
    {

        static internal void accountPrinting(ref DataTable dtAccount)
        {

            Console.WriteLine("{0,15}{1,15}{2,15}{3,15}{4,15}",
                             dtAccount.Columns[0].ColumnName,
                             dtAccount.Columns[3].ColumnName,
                             dtAccount.Columns[1].ColumnName,
                             dtAccount.Columns[2].ColumnName,
                             dtAccount.Columns[4].ColumnName);

            foreach (DataRow dr in dtAccount.Rows)
            {
                Console.WriteLine("{0,15}{1,15}{2,15}{3,15}{4,15}",
                                   dr[0],
                                   dr[3].ToString(),
                                   dr[1],
                                   "$" + dr[2].ToString(),
                                   "$" + dr[4].ToString()
                                   );
            }

        }

        static internal void accountStarting(ref DataTable dt)
        {

            //accounttype: Checking = C; Investment Individual = II;  Investment Corporate = IC

            dt = new DataTable();
            dt.Columns.Add("sAccountOwner", typeof(System.String));
            dt.Columns.Add("sAccountType", typeof(System.String));
            dt.Columns.Add("fBalance", typeof(System.Decimal));
            dt.Columns.Add("iAccountID", typeof(int));
            dt.Columns.Add("fBalanceStart", typeof(System.Decimal));
            dt.AcceptChanges();

            DataRow dr = dt.NewRow();
            dr[0] = "Acme Plumbing"; dr[1] = "C"; dr[2] = 1111; dr[3] = 1; dr[4] = dr[2];
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr[0] = "Acme Plumbing"; dr[1] = "IC"; dr[2] = 1111; dr[3] = 2; dr[4] = dr[2];
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr[0] = "Joe Jones"; dr[1] = "II"; dr[2] = 2222; dr[3] = 3; dr[4] = dr[2];
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr[0] = "Alice Smith"; dr[1] = "I"; dr[2] = 3333; dr[3] = 4; dr[4] = dr[2];
            dt.Rows.Add(dr);
            dt.AcceptChanges();
        }


        static internal bool accountLimit(ref DataRow drAccount, ref string sTransactionType, ref decimal fTransactionAmount)
        {
            switch (drAccount["sAccountType"].ToString())
            {
                case "II":
                    if (sTransactionType == "Withdrawal" && fTransactionAmount > 500.00M) return false;
                    break;
                default:
                    break;
            }
            return true;  
        }

    }//end cAccount=======================================================================





}//end namepace

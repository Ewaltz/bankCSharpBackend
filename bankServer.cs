using System;
using System.Collections.Generic;
using System.Globalization;
using System.Collections;

namespace Bank
{
    class bankServer
    {
        public static Hashtable userList; /// store a table of users (userName, User<object>)
        public const int MINNAMELENGTH = 5;
        public const int MINPASSLENGTH = 5;

        public static int Main(string[] args)
        {
            string input;
            //testUser();
            //testTable();
            userList = new Hashtable();
            Console.WriteLine("Welcome");
            Console.WriteLine("/help for more information");

            do
            {
                Console.WriteLine("How may I help you?");
                input = Console.ReadLine();
                if(input == "serverShut3324") //random shutdown sequence
                {
                    Console.WriteLine("Server Shutting Down, Good Bye.");
                    testTable();
                    break;
                }else if (input == "/create")
                {
                    User created = accountCreate();
                    if(created != null){
                        userList.Add(created.UserName, created);
                    }
                }else if(input == "/login")
                {
                    loginAction();
                }else if(input == "/help")
                {
                    helpMessages("mainMenu");
                }else
                {
                    Console.WriteLine("Sorry, did not understand command");
                }

            }while(true); 
            
            return 0; 

        }

        /// Display available commands for user
        private static void helpMessages(string location)
        {
            //throw new NotImplementedException();
            string[] messages;
            if(location == "mainMenu"){
                messages = new string[] { "\t/create -registers a new account",
                                        "\t/login - login user",
                                        "\t/help - display usable commands" 
                                        };
            }else{
                messages = new string[] { "\t/deposit -deposit money to account",
                                        "\t/withdraw -withdraw availble funds from account",
                                        "\t/checkBalance -check amout of funds in account",
                                        "\t/history -display transaction history",
                                        "\t/logout -logout user",
                                        "\t/help -display available actions"
                };
            }
            Console.WriteLine("Help Menu:");
            foreach(string i in messages){
                Console.WriteLine(i);
            }          
        }

        /// Login Sequence
        private static void loginAction()
        {
            string uName;
            string uPass;
            Console.WriteLine("username?");
            uName = Console.ReadLine();
            Console.WriteLine("password?");
            uPass = Console.ReadLine();
            User loggedInUser = checkLogin(uName, uPass);
            if(loggedInUser != null){
                loggedInActions(loggedInUser);
            }
        }

        /// User Interaction when logged in
        private static void loggedInActions(User loggedInUser)
        {
            string input;
            Console.WriteLine("\nlogged in as user {0} \n", loggedInUser.UserName);

            while(true){
                Console.WriteLine("what would you like to do?");
                input = Console.ReadLine();
                if(input =="/deposit"){
                    handleTransactions(loggedInUser, "deposit");
                }else if(input=="/withdraw"){
                    handleTransactions(loggedInUser, "withdraw");
                }else if(input == "/checkBalance"){
                    Console.WriteLine("You({0}) have a balance of ${1}",loggedInUser.UserName,loggedInUser.checkBalance());
                }else if (input == "/history"){
                    displayUserTransactionHistory(loggedInUser);
                }else if(input == "/help"){
                    helpMessages("loginMenu");
                }else if (input == "/logout"){
                    Console.WriteLine("User {0} logged out\n", loggedInUser.UserName);
                    break;
                }
                else{
                    Console.WriteLine("Sorry, did not understand that");
                }
            }
        }

        /// Validate money input format
        private static double validAmount(string value)
        {
            //Check for valid currency value, $*.xx
            double moneyAmount;
            try{
                moneyAmount = double.Parse(value);
                if(double.TryParse(value, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out moneyAmount)){
                    if(moneyAmount < 0){
                        Console.WriteLine("Invalid Amount, Try again.");
                        return -1;
                    }else{
                        //check for 2 decimal values
                        double temp = moneyAmount* 100; 
                        if((int)temp == temp){
                            return moneyAmount;
                        }
                        Console.WriteLine("Invalid Amount, Try again.");
                        return -1;
                    }
                }else{
                    Console.WriteLine("Invalid Amount, Try again.");
                    return -1;
                }
            }catch(Exception e){
                Console.WriteLine(e.ToString());
            }
            return -1;
        }

        /// Handler Deposit and Withdraw transactions
        private static void handleTransactions(User loggedInUser, string action){
            double moneyAmount;
            Console.WriteLine("How much would you like to {0}?", action);
            try{
                moneyAmount = validAmount(Console.ReadLine());
                if(moneyAmount == -1){
                    return;
                }
                if(action == "deposit"){
                    loggedInUser.deposit(moneyAmount);
                }else if(action == "withdraw"){
                    if(loggedInUser.withdraw(moneyAmount) == -1){
                        Console.WriteLine("Not enough funds in account for this action");
                        return;
                    }
                }else{
                    Console.WriteLine("Invalid, should not be here. Nothing Done.");
                    return;
                }
                Console.WriteLine("Your new Balance is ${0}", loggedInUser.checkBalance());       
            }catch(Exception e){
                Console.WriteLine(e.ToString());
            }
        }
        /// Method for displaying user transaction history
        private static void displayUserTransactionHistory(User loggedInUser)
        {
            var userHistory = loggedInUser.transHistory();
            for(int i = 0; i< userHistory.Item1.Count; i++)
            {
                Console.WriteLine("{0} | {1}\t| ${2}",userHistory.Item1[i],userHistory.Item2[i], userHistory.Item3[i]);
            }
        }

        /// Check login credentials 
        private static User checkLogin(string uName, string uPass)
        {
            //check name,check pass
            if(userList.ContainsKey(uName)){
                User u = (User)userList[uName];
                // if user exists, checkpass
                if(u.checkPass(uPass)){
                    return u;
                }
                Console.WriteLine("Username or Password Incorrect");
                return null;
            }else{
                //failed
                Console.WriteLine("Username or Password Incorrect");
                return null;
            }
        }

        /// Handle user account creation
        private static User accountCreate()
        {
            string uName;
            string uPass;
            Console.WriteLine("What is the user name of the account");
            uName = Console.ReadLine();
            Console.WriteLine("What will the password be?");
            uPass = Console.ReadLine();
            //check username and password lengths
            if(userList.ContainsKey(uName)){  ///User already exist
                Console.WriteLine("Account creation Failed, invalid username and/or password");
                return null;
            }else{
                ///simple check for a min length, could later expand to do more robust requirment checks 
                if(uName.Length < MINNAMELENGTH || uPass.Length < MINPASSLENGTH){  
                    Console.WriteLine("Account creation Failed, invalid username and/or password");
                    return null;
                }
            }


            try{
                User newUser= new User(uName, uPass);
                Console.WriteLine("user {0} created", uName);
                return newUser;
            }catch(Exception e){
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        /// Testing purpose, testing User class
        public static void testUser(){
            User one = new User("name1", "pass1");
            Console.WriteLine("wrongName Check:{0}, RightName:{1}",one.checkName("u1"), one.checkName("name1"));
            Console.WriteLine("wrongPass: {0}",one.checkPass("pass2"));
            one.passwordChange("pass2");
            Console.WriteLine("RightPass: {0}",one.checkPass("pass2"));
            one.deposit(500.00);
            Console.WriteLine(one.checkBalance());
            Console.WriteLine(one.withdraw(20.00));
            Console.WriteLine(one.withdraw(2000.00));
            one.deposit(50000.00);
            Console.WriteLine(one.checkBalance());
            var t = one.transHistory();
            
            for(int i = 0; i< t.Item1.Count; i++)
            {
                Console.WriteLine("{0} | {1}\t| ${2}",t.Item1[i],t.Item2[i], t.Item3[i]);
            }
        }

        /// Testing Purpose, checking userList table
        public static void testTable(){
            ICollection key = userList.Keys;
         
         foreach (string k in key) {
             User test = (User)userList[k];
             test.deposit(33.5);
            Console.WriteLine(k + ": $" +  test.checkBalance());
         }
        }
    }
}

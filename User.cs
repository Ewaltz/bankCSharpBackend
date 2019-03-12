using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Bank
{
    public class User
    {
        private string userName;
        private string userPassword;
        private double funds;
        //User History (actionDate, action, amount)
        private List<string> actionDate;
        private List<string> actions;
        private List<double> amounts;

        public string UserName { get => userName; set => userName = value; }

        public User(string name, string pass)
        {
            userName = name;
            userPassword = hashPassword(pass);
            funds = 0;
            actions = new List<string>();
            amounts = new List<double>();
            actionDate = new List<string>();
        }
        public double checkBalance() => funds;

        public Boolean checkName(string name){
            if(name == userName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Boolean checkPass(string pass){
            
            if (checkHashPassword(pass))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string passwordChange(string newPass)
        {
            //should check for previous password lists
            //and make sure it's a new password; maybe some other time
            userPassword  = hashPassword(newPass);
            return "Password Changed";
        }

        public double withdraw(double amount) 
        {
            if(amount > funds)
            {
                return -1;
            }
            else
            {
                funds -= amount;
                actions.Add("Withdraw");
                amounts.Add(amount);
                actionDate.Add(DateTime.Now.ToShortDateString() +" " +DateTime.Now.ToShortTimeString());
                return amount;
            }
        }

        public void deposit(double amount){
            funds += amount;
            actions.Add("Deposit");
            amounts.Add(amount);
            actionDate.Add(DateTime.Now.ToShortDateString() +" " +DateTime.Now.ToShortTimeString());
        }
        public Tuple<List<string>, List<string>, List<double>> transHistory(){
            return Tuple.Create(actionDate, actions, amounts);
        }

        private static string hashPassword(string uPass){
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(uPass, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            //combine everything into hasBytes
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        private Boolean checkHashPassword(string testPass){
            /* Fetch the stored value */
            //string savedPasswordHash = userPassword;
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(userPassword);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(testPass, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i=0; i < 20; i++)
                if (hashBytes[i+16] != hash[i]){
                    //throw new UnauthorizedAccessException();
                    return false;
                }

            return true;
        }
    }
}
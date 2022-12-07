using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Threading.Tasks;
using FastFoodTelegramBot.Repositories;
using FastFoodTelegramBot.Models;
using FastFoodTelegramBot.Commands;
using static FastFoodTelegramBot.CommandNames;
using System.Reflection;
using System.Reflection.Emit;
using FastFoodTelegramBot.Utilities;
using FastFoodTelegramBot.Services;
using Microsoft.EntityFrameworkCore;
using FastFoodTelegramBot.Init;

namespace FastFoodTelegramBot.Init
{
    class CallBackFunc
    {
        public static string[] Args { get; set; }

        public delegate Account DelegateGetByID(List<Account> list, long id, out bool isFound);
        public static DelegateGetByID @GetByID;

        public static Action<Account, List<Account>> @AddItemInDb;
        public static Action<Order<ShopCartItem<Product>, Product>, List<Order<ShopCartItem<Product>, Product>>> @AddOrderInDb;
        public static Func<List<Product>, List<Product>>? @LoadProdFileToList;

        public delegate void DelegateSaveListToFile(List<Product> list);
        public static DelegateSaveListToFile @SaveProdListToFile;

        public delegate bool DelegateUpdAccount(Account acc, List<Account> list);
        public static DelegateUpdAccount @UpdateAccount;

        public delegate Task DelegateMail(Account foundAccount, string subj, string mailbody);
        public static DelegateMail @EmailSend;
        public static void Init()
        {
            CommandNames.CmdArg = "list";
            @AddItemInDb = (acc, list) => ListAccService<Account>.AddItemInDb(acc, list);
            @AddOrderInDb = (order, list) => list.Add(order);
            @GetByID = ListAccService<Account>.GetByID;
            @LoadProdFileToList = (list) => list;
            @SaveProdListToFile = delegate (List<Product> list)
            {
                JsonService<Product>.SaveListToFile(list, CommandNames.JsonProdPath);
                using ApplicationContext<Product> db = new ApplicationContext<Product>(CommandNames.ProdConnectionDB); //DB context.
                DbService<Product>.SaveListToDB(list, db);
                //db.Dispose();
            };
            @UpdateAccount = (Account acc, List<Account> list) => (true);
            @EmailSend = (foundAccount, subj, mailbody) => EmailSender.SendEmailAsync(foundAccount, subj, mailbody);

            foreach (string arg in Args)
            {
                if (arg.Equals("j", StringComparison.OrdinalIgnoreCase))
                {
                    CommandNames.CmdArg = "json";
                    @AddItemInDb = (acc, list) => JsonService<Account>.AddItemInDb(acc, list, CommandNames.JsonAccPath);
                    @AddOrderInDb = (order, list) => JsonService<Order<ShopCartItem<Product>, Product>>.AddItemInDb(order, list, CommandNames.JsonOrderPath);
                    @LoadProdFileToList = (list) => JsonService<Product>.LoadFileToList(list, CommandNames.JsonProdPath);
                    @SaveProdListToFile = delegate (List<Product> list) { };
                    @UpdateAccount = (acc, list) => JsonService<Account>.UpdateItem(acc, list, CommandNames.JsonAccPath);
                    @GetByID = delegate (List<Account> list, long id, out bool isFound)
                    {
                        return JsonService<Account>.GetByID(list, CommandNames.JsonAccPath, id, out isFound);
                    };
                }
                if (arg.Equals("db", StringComparison.OrdinalIgnoreCase))
                {
                    CommandNames.CmdArg = "db";
                    @AddItemInDb = delegate (Account acc, List<Account> list)
                    {
                        using ApplicationContext<Account> db = new ApplicationContext<Account>(CommandNames.AccConnectionDB);//DB context.
                        DbService<Account>.AddItemInDb(acc, db);
                    };
                  
                    @AddOrderInDb = delegate (Order<ShopCartItem<Product>, Product> order, List<Order<ShopCartItem<Product>, Product>> list)
                    {
                        using ApplicationContext<Order<ShopCartItem<Product>, Product>> db = new ApplicationContext<Order<ShopCartItem<Product>, Product>>(CommandNames.OrderConnectionDB);  //DB context.                                                                                                                                                                            
                        DbService<Order<ShopCartItem<Product>, Product>>.AddItemInDb(order, db);
                    };

                    @LoadProdFileToList = delegate (List<Product> list)
                    {
                        using ApplicationContext<Product> db = new ApplicationContext<Product>(CommandNames.ProdConnectionDB);//DB context.
                        List<Product> lst = DbService<Product>.LoadDBToList(list, db);
                        return lst;
                    };

                    @SaveProdListToFile = delegate (List<Product> list) { };

                    @UpdateAccount = delegate (Account acc, List<Account> list)
                    {
                        using ApplicationContext<Account> db = new ApplicationContext<Account>(CommandNames.AccConnectionDB); //DB context.
                        bool isUpdated = DbService<Account>.UpdateItem(acc, db);
                        return isUpdated;
                    };

                    @GetByID = delegate (List<Account> list, long id, out bool isFound)
                    {
                        using ApplicationContext<Account> db = new ApplicationContext<Account>(CommandNames.AccConnectionDB); //DB context.
                        Account acc = DbService<Account>.GetByID(db, id, out isFound);
                        return acc;
                    };
                }
                if (arg.Equals("l", StringComparison.OrdinalIgnoreCase))
                {
                    CommandNames.CmdArg = "list";
                    @AddItemInDb = (acc, list) => ListAccService<Account>.AddItemInDb(acc, list);
                    @AddOrderInDb = (order, list) => list.Add(order);
                    @GetByID = ListAccService<Account>.GetByID;
                    @LoadProdFileToList = (list) => list;
                    @SaveProdListToFile = delegate (List<Product> list)
                    {
                        JsonService<Product>.SaveListToFile(list, CommandNames.JsonProdPath);
                        using ApplicationContext<Product> db = new ApplicationContext<Product>(CommandNames.ProdConnectionDB); //DB context.
                        DbService<Product>.SaveListToDB(list, db);   
                    };
                    @UpdateAccount = (Account acc, List<Account> list) => (true);  //ListAccService<Account>.UpdateAccount;
                }

                if (arg.Equals("dl", StringComparison.OrdinalIgnoreCase)) CommandNames.LogLevel = "debug";
                if (arg.Equals("il", StringComparison.OrdinalIgnoreCase)) CommandNames.LogLevel = "info";
                if (arg.Equals("el", StringComparison.OrdinalIgnoreCase)) CommandNames.LogLevel = "error";

                if (arg.Equals("sg", StringComparison.OrdinalIgnoreCase))
                {
                    @EmailSend = (foundAccount, subj, mailbody) => EmailSendGrid.SendEmailAsync(foundAccount, subj, mailbody);
                    CommandNames.EmailService = "SendGrid";
                }
            }
        }
    }
}

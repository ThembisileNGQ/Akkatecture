# Akkatecture Walkthrough Sample

This is the sample that shows the completed result of the [walkthrough](https://akkatecture.net/docs/walkthrough-introduction).

# Akkatecture.Walkthrough.Domain

This is the domain of the sample which follows from the walkthrough tutorial.

### Commands
* **OpenNewAccountCommand** - Command for creating a new bank account.
* **ReceiveMoneyCommand** - Command for adding funds to a bank account.
* **TransferMoneyCommand** - Command for sending funds from a bank account.

### Events
* **AccountOpenedEvent** - Event that denotes an account has been opened.
* **FeesDeductedEvent** - Event that denotes that bank fees have been deducted from an account.
* **MoneyReceivedEvent** - Event that denotes that a mank account has received funds.
* **MoneySentEvent** - Event that denotes that a bank account has sent funds.
* **MoneyTransferCompletedEvent** - Event that denotes that a transfer has ended.
* **MoneyTransferStartedEvent** - Event that denotes that a money transfer has started.

# Akkatecture.Walkthrough.Application


> to run the application in jetbrains rider or visual studio code, run the `Akkatecture.Walkthrough.Application` configuration in the IDE.
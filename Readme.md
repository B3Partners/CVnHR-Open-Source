# CVnHR

This project is commissioned by "Provincie Drenthe" and provides an interface between 
third party applications used in the province and the Dutch Chamber of Commerce (KvK).
The application allows for storage of records (inschrijvingen) and provides a simple search
service. Furthermore the application provides batchwise updating and synchronisation.

__Open Source__: The project is made open source to allow other provinces to use this 
application either stand-alone or in combination with the BRMO application by B3partners

Original development is done by QNH Consulting B.V.

__Contact__ Corné Hogerheijde @ QNH or B3Partners (TODO)


## Development environment setup

Get your development environment up and running by cloning this repo, get a government 
PKI certificate and arrange a subscription to the HR-Dataservice of the Dutch Chamber 
of Commerce (KvK).
- Run visual studio as an administrator
- Copy config files from Config/examples and fill out with necessary values
- Install the PKI certificate and fill out the values needed in the .config files
- Create a database on either Oracle or SQL and update .config files accordingly
- Surfs up dude!

## README "QNH.Overheid.KernRegister.Beheer"
APP_DATA and Logs folders need read and write permissions for Elmah logging.
# CVnHR

This project is commissioned by "Provincie Drenthe" and provides an interface between 
third party applications used in the province and the Dutch Chamber of Commerce (KvK).
The application allows for storage of records (inschrijvingen) and provides a simple search
service. Furthermore the application provides batchwise updating and synchronisation.

__Open Source__: The project is made open source to allow other provinces to use this 
application either stand-alone or in combination with the BRMO application by B3partners

Original development is done by QNH Consulting B.V.

__Contact__ Corné Hogerheijde @ QNH


## Development environment setup

Get your development environment up and running by cloning this repo, get a government 
PKI certificate and arrange a subscription to the HR-Dataservice of the Dutch Chamber 
of Commerce (KvK).
- Run visual studio as an administrator
- Ensure a Web.debug.config and web.release.config are created (TODO create default 
way to easy debug and release with sensitive info without uploading to source control)
- Install the PKI certificate and fill out the values needed in the app- and web.config
- Update the web.config (connectionstrings, appsettings and system.servicemodel)
- Create a database on either Oracle or SQL and update app- and web.config accordingly
- Surfs up dude!

## README "QNH.Overheid.KernRegister.Beheer"
APP_DATA and Logs folders need read and write permissions for Elmah logging.
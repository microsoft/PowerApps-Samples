# Metadata Visualizer

![Entity diagram example](./images/intro-graphic.png)

Metadata Visualizer (MetaViz) downloads the metadata of your Dynamics 365 Customer Engagement or Common Data Service organization to visually display the entities, entity relationships, and registered plug-in information.

## What can you do with MetaViz?

The application can download entity, plug-in, and custom workflow activity metadata information from your organization into a text file and you can browse that data offline.
Since this metadata information is downloaded into readable files you can: keep it in a repo, compare metadata of different environments, track changes to metadata, let somebody check the information without giving them access to the organization, and more.

Below is a description of the files that are generated when you download an organization's metadata.

- Trigger definitions (HTML file)

Registered synchronous and asynchronous plug-ins and custom workflow activities (and their filtering attributes) are dumped into the html file.
You can find what activity or plug-in is registered on an entity to trace what code will be triggered when a core system operation (create, update, delete, etc.) occurs.

- Entity definitions (text file)

Localized/customized display names for all attributes, descriptions, and data-types across all entities are dumped into a text file.
You can compare the scheme across environments and/or releases easily by using your preferred text comparison (differences) tools.

- Entity relationship diagram (JSON file)

The ER Viewer allows you to visually browse the relationships across your chosen entities.
You can select specific entities and check their relationships in a visual diagram.

## How to use the application

### Download Metadata from the Online organization

Type your **organization URL** (e.g. https://contoso.crm.dynamics.com) in URL field then click **Download Metadata** button.

### ALl entities checkbox

Tick this checkbox if Entity definitions and ER diagram reports are required. This option needs to download all entities metadata therefore execution will take longer.

### ER (Entity Relationship) viewer

You can browse the downloaded Entity Relationship Json file using the ER viewer. 

1. Click **ER Viewer** button. From **Diagram - Open ER Json file** menu the json file downloaded.
2. After entities are loaded choose **Diagram - Draw Selected Entities** menu to render the diagram.
3. **Click the entity name** to display the scheme information and its trigger registrations.
4. **Hover the mouse pointers on relations** to see the relation information.

Please check FAQ sections for further information how ER viewer works.

## FAQs

**Q. How can I limit the entities rendered in the ER diagram?**

You need to select a list of entities you want to use.

1. Choose **Entities - Clear** to untick all entities.
2. Tick the entities you are interested.
3. Use either **Diagram - Draw Selected Entities** or **Draw Related Entities** menu. 

**Draw Selected Entities** menu automatically expands the selection to the related entities to the ones currently selected.
You can copy and paste the list of entities selected by **Entities - Copy** and **Entities - Paste** menu.

**Q. How to add related entities to/remove the entity from the diagram?**

Click the Entity in the diagram. Then choose **Selected Entity - Remove** menu. Same way you can perform different operations such as **Select Related Entities** on the selected entity as well.

**Q. What library is used to render the ER diagram?**

[Microsoft Automatic Graph Layout](https://www.microsoft.com/en-us/research/project/microsoft-automatic-graph-layout/) (MSAGL). *MSAGL is a .NET tool for graph layout and viewing. It was developed in Microsoft by Lev Nachmanson, Sergey Pupyrev, Tim Dwyer and Ted Hart.*

**Q. What are the mysterious 3 alphabet letters dumped in the scheme text file?**

They are the first 3 letters of the entity name hashcode. These letters can help text comparison tools to find matching rows correctly.
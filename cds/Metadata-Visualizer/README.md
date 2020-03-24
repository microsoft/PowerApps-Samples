# Metadata Visualizer (MetaViz)

MetaViz downloads the metadata of your Dynamics 365 CE organization to render the entity, trigger and entity relation information.

## What can you do with MetaViz?

You can download various Metadata information from Dynamics 365 CE organization and browse them offline.
Since information can be downloaded into the static files you can keep them in the repo, compare different environment, track changes, let somebody check the information without giving an access to the organization, etc. easily and quickly.

- Trigger definitions (html file)

Registered sync and async plug-ins and workflows and their filtering attributes, etc. are all dumped into html file.
You can find what workflow/plugin are registered on the entity to trace what code will be triggered when CRUD operation occurrs.

Following reports will be available if **All entities** option is selected.

- Entity definitions (txt file)

Localized/customized display names for all attributes, description and datatype across all entities are dumped into the text format.
You can compare the scheme across environments and/or releases easily by your preferred text comparison tools.

- ER diagram (json file)

ER (Entity relationship) viewer allows you to browse the relationship across your selected entities.
You can select some entities and check their relationship in the visualization.

## How to download Metadata using MetaViz

#### Download Metadata from the Online organization

Type your **organization URL** (e.g. https://contoso.crm.dynamics.com) in URL field then click **Download Metadata** button.

#### ALl entities checkbox

Tick this checkbox if Entity definitions and ER diagram reports are required. This option needs to download all entities metadata therefore execution will take longer.

## ER (Entity Relationship) viewer

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
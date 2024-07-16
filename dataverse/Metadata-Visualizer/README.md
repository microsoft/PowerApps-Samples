# Metadata Visualizer

:::image type="content" source="images/intro-graphic.PNG" alt-text="Diagram of an example of entity relationships that this sample produces.":::

Metadata Visualizer (MetaViz) downloads the metadata of your Dynamics 365 Customer Engagement or Microsoft Dataverse organization to visually display the entities, entity relationships, and registered plug-in information.

## What can I do with MetaViz?

The application can download entity, plug-in, and custom workflow activity metadata information from your organization into a text file and you can browse that data offline.

Since this metadata information is downloaded into readable files you can: keep it in a repo, compare metadata of different environments, track changes to metadata, let somebody check the information without giving them access to the organization, and more.

### Files generated

The following files are generated when you download an organization's metadata.

- **Trigger definitions (HTML file)**

  Registered synchronous and asynchronous plug-ins and custom workflow activities (and their filtering attributes) are dumped into the html file.

  You can find what activity or plug-in is registered on an entity to trace what code is triggered when a core system operation (create, update, delete, etc.) occurs.

- **Entity definitions (text file)**

  Localized/customized display names for all attributes, descriptions, and data-types across all entities are dumped into a text file.

  You can compare the scheme across environments and/or releases easily by using your preferred text comparison (differences) tools.

- **Entity relationship (ER) diagram (JSON file)**

  The ER Viewer window that you see when you run the application allows you to visually browse relationships across your chosen entities. You can select specific entities and check their relationships in a visual diagram.

## How to use the application

### Build and run the application

1. Load the solution into Visual Studio, build, and then run the program.
2. After the download dialog is displayed, enter your target organization URL in the provided field of the dialog.
3. When prompted, specify a folder where you want to store the organization's metadata.
4. When prompted, provide your organization sign-in information.
5. The organization metadata download starts. The download might take several minutes.
6. The entity relationship (ER Viewer) window opens and displays an initial (default) diagram of the pre-selected entities.
7. Resize the window as appropriate.
8. Select the zoom (+) icon in the toolbar or scrollwheel on your mouse to enlarge the diagram and the pan (hand) icon to pan around the view.
9. Hover the cursor over the other icons in the toolbar to see what other functionality is available.

### Change the entity diagram

1. In the ER Viewer, select **Entities** > **Clear**.
2. In the left panel, select any entities you want see relationships of in the diagram, for example: account, contact, and activityparty.
3. Select **Diagram** > **Draw Selected Entities** to view these entities and their relationships.
4. Next, select **Diagram** > **Draw Related Entities** to view all other entities that have a relationship with the entities specified in step #2.

:::image type="content" source="images/er-viewer.PNG" alt-text="Screenshot that shows entity relationships in the ER Viewer window.":::

### View entity metadata

1. In the ER Viewer, hover the cursor over an entity in the entity diagram to view a summary description.
2. Select the entity in the diagram to display the Schema Viewer showing the entity's metadata like attributes and relationships.

   :::image type="content" source="images/schema-viewer.PNG" alt-text="Screenshot that shows the Schema Viewer, a window that appears from running the application and selecting an entity in the ER Viewer.":::
3. Select another entity in the diagram to now view its metadata in the Schema Viewer window.
4. Sign in to your organization using your default internet browser.
5. In the ER Viewer window, select an entity and choose **Selected Entity** > **Copy URL**.
6. Paste the URL into your browser to see the list of records for that entity.
   > [!NOTE]
   > You might need to choose a different view in the browser page other than the default view to see the records for that entity.

### View plug-in and custom activity registrations

1. Sign in to an organization in your default internet browser.
2. In the ER Viewer, select **Diagram** > **Trigger information**. A browser window or tab opens displaying entity information and registered plug-in or custom workflow activity information.
3. Select an entity link to jump to the plug-in or custom activity information for that entity.
4. Select other links to see what kind of information is available from that browser page.

:::image type="content" source="images/trigger-view.PNG" alt-text="Screenshot of the trigger information page in your web browser.":::

## FAQs

### Can I choose the entities in the ER Viewer diagram?

You can select a list of entities you want to use.

1. Choose **Entities > Clear** to de-select all entities.
2. Select (check) the entities you're interested in viewing.
3. Use either **Diagram > Draw Selected Entities** or the **Draw Related Entities** menu items.

   The **Draw Selected Entities** menu automatically expands the selection to the related entities to the ones currently selected.

You can copy and paste the list of entities selected by choosing **Entities > Copy** and **Entities > Paste** menu items.

### How do I add or remove related entities from the diagram?

Select (check) the entity in the diagram and then choose the **Selected Entity > Remove** menu item. In the same way you can perform different operations such as **Select Related Entities** on the selected entity.

### What library is used to render the ER Viewer diagram?

[Microsoft Automatic Graph Layout](https://www.microsoft.com/research/project/microsoft-automatic-graph-layout/) (MSAGL).

*MSAGL is a .NET tool for graph layout and viewing. It was developed in Microsoft by Lev Nachmanson, Sergey Pupyrev, Tim Dwyer and Ted Hart.*

## Version history

04-06-2021

- Added the global plug-in in the report
- Added the workflow activities registration status in the reports
- Improved the registration status message
- Added the categories for plug-in and workflows in the reports

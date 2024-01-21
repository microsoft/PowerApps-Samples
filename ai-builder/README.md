# AI Builder Samples

This topic provides information about the sample data files available in the **ai-builder** folder.

AI Builder documentation is available here: <https://learn.microsoft.com/ai-builder>

## Sample data for prediction

To help you get started quickly with prediction, use these three sample datasets. For information on how to import the sample data and use it in a prediction model, see [Data Preparation](https://learn.microsoft.com/ai-builder/prediction-data-prep) in AI Builder docs.

Please note that you do not need to download the CSV files, but rather link the raw URL of the CSV files during the data import flow.

### Online Shopper Intention

This dataset is from the UCI Machine Learning Repository. The prediction scenario is whether a customer will make a purchase using web analytics data from an online retailer.
-	**Solution**: AIBuilderOnlineShopperIntention_1_0_0_0.zip
-	**Data**: aib_onlineshopperintention.csv

*Citation*: Sakar, C.O., Polat, S.O., Katircioglu, M. et al. Neural Comput & Applic (2018). <https://doi.org/10.1007/s00521-018-3523-0> 

### Steel Plates Fault

This dataset is from the UCI Machine Learning Repository. The prediction scenario is whether a stainless steel plate has a surface defect using indicators that approximate shape and reflective properties of the steel plate.
-	**Solution**: AIBuilderSteelPlatesFault_1_0_0_0.zip
-	**Data**: aib_steelplatesfault.csv

*Citation*: Dataset provided by Semeion, Research Center of Sciences of Communication, Via Sersale 117, 00128, Rome, Italy. <http://www.semeion.it/>

### Brazilian E-Commerce Public Dataset 

This is a Brazilian ecommerce public dataset of orders made at Olist Store <https://www.olist.com>. The dataset has information of 100k orders from 2016 to 2018 made at multiple marketplaces in Brazil. We did some modifications on the dataset by re-sampling, and selecting a more homogeneous subset to help avoid creating an [overfit model](/ai-builder/manage-model#overfit-models).
-	**Solution**: BrazilianCommerce_1_0_0_4_managed.zip
-	**Data**: order.csv, customer,csv, product.csv

*Citation*: Dataset provided by Olist Store, <https://www.olist.com>

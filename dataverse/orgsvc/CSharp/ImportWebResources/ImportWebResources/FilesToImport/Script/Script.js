
function getXMLDoc(file) {
 if (typeof window.ActiveXObject != "undefined") {
  var axo = new ActiveXObject("Msxml2.DOMDocument.6.0");
  axo.async = false;
  axo.load(file);
  return axo;
 }
 else {
  xhttp = new XMLHttpRequest();
 }
 xhttp.open("GET", file, false);
 xhttp.send("");
 return xhttp.responseXML;
}

function showData() {
 xml = getXMLDoc("Data/Data.xml");
 xsl = getXMLDoc("XSL/Transform.xsl");
 // Internet Explorer
 if (typeof window.ActiveXObject != "undefined") {
  ex = xml.transformNode(xsl);
  document.getElementById("results").innerHTML = ex;
 }
 //  Firefox, Chrome, & Safari
 else if (document.implementation && document.implementation.createDocument) {
  xsltProcessor = new XSLTProcessor();
  xsltProcessor.importStylesheet(xsl);
  results = xsltProcessor.transformToFragment(xml, document);
  document.getElementById("results").appendChild(results);
 }
}
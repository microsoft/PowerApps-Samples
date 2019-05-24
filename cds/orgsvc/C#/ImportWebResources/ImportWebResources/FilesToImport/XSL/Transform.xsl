<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl"
>
 <xsl:output method="html" omit-xml-declaration="yes" />

 <xsl:template match="@* | node()">
  <xsl:copy>
   <xsl:apply-templates select="@* | node()"/>
  </xsl:copy>
 </xsl:template>


 <xsl:template match="people">
  <xsl:element name="table">
   <xsl:attribute name="summary">
    This table displays data from the sample file Data/data.xml contains list of First Name and Last Name.
   </xsl:attribute>
   <xsl:element name="thead">
    <xsl:element name="tr">
     <xsl:element name="th">
      <xsl:text>First Name</xsl:text>
     </xsl:element>
     <xsl:element name="th">
      <xsl:text>Last Name</xsl:text>
     </xsl:element>
    </xsl:element>
   </xsl:element>
   <xsl:element name="tbody">
    <xsl:apply-templates />
   </xsl:element>
  </xsl:element>

 </xsl:template>

 <xsl:template match="person">
  <xsl:element name="tr">
   <xsl:element name="td">
    <xsl:value-of select="@firstName"/>
   </xsl:element>
   <xsl:element name="td">
    <xsl:value-of select="@lastName"/>
   </xsl:element>
  </xsl:element>

 </xsl:template>

</xsl:stylesheet>

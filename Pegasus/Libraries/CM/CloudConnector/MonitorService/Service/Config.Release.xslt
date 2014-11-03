<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>

  <!-- Default template -->
  <xsl:template match="node()|@*">
    <xsl:copy>
      <xsl:apply-templates select="node()|@*"/>
    </xsl:copy>
  </xsl:template>

  <!-- system.diagnostics replacement template -->
  <xsl:template match="/configuration/system.diagnostics">
  </xsl:template>

  <!-- configSecions replacement template -->
  <xsl:template match="/configuration/configSections">
  </xsl:template>

</xsl:stylesheet>
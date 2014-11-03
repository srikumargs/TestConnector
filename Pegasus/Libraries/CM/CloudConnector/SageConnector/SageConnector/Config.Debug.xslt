<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:param name="SandboxDir"></xsl:param>
  
  <xsl:output method="xml" indent="yes"/>

  <!-- Default template -->
  <xsl:template match="node()|@*">
    <xsl:copy>
      <xsl:apply-templates select="node()|@*"/>
    </xsl:copy>
  </xsl:template>

  <!-- system.diagnostics replacement template -->
  <!--<xsl:template match="/configuration/system.diagnostics">
    <system.diagnostics>
      <sources>
        <source name="System.ServiceModel"
                switchValue="Verbose, ActivityTracing"
                propagateActivity="true">
          <listeners>
            <add name="traceListener"
                type="System.Diagnostics.XmlWriterTraceListener">
              <xsl:attribute name="initializeData"><xsl:value-of select="$SandboxDir"/>\Debug.SageConnector.svclog</xsl:attribute>
            </add>
          </listeners>
        </source>
      </sources>
    </system.diagnostics>
  </xsl:template>-->

</xsl:stylesheet>
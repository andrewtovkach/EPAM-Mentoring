﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:cd="http://library.by/catalog"
                extension-element-prefixes="cd">
  
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/">
    <rss version="2.0" xmlns:atom="http://www.w3.org/2005/Atom">
      <xsl:apply-templates />
    </rss>
  </xsl:template>
  
  <xsl:template match="cd:catalog">
    <channel>
      <title>Books catalog</title>
      <description>RSS tape</description>
      <link>http://library.by/catalog</link>
      <language>ru-ru</language>
      <lastBuildDate>
        <xsl:value-of select="format-date(current-date(),'[D01] [MNn, *-3] [Y0001] 12:00:00 GMT')"/>
      </lastBuildDate>
      <xsl:apply-templates />
    </channel>
  </xsl:template>

  <xsl:template match="cd:catalog/cd:book">
    <item>
      <title>
        <xsl:value-of select="cd:title"/>
      </title>
      <xsl:if test="(cd:isbn != 'minOccurs') and (cd:genre = 'Computer')">
        <link>
          <xsl:value-of select="concat('http://my.safaribooksonline.com/', cd:isbn, '/')"/>
        </link>
      </xsl:if>
      <description>
        <xsl:value-of select="cd:description"/>
      </description>
      <author>
        <xsl:value-of select="cd:author"/>
      </author>
      <guid>
        <xsl:value-of select="@id"/>
      </guid>
      <pubDate>
        <xsl:value-of select="format-date(cd:publish_date,'[D01] [MNn, *-3] [Y0001] 12:00:00 GMT')"/>
      </pubDate>
    </item>
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>

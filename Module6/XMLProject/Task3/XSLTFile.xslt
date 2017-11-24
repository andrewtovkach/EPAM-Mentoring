<xsl:stylesheet version="2.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:cd="http://library.by/catalog"
                extension-element-prefixes="cd">

  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <HTML>
      <HEAD>
        <TITLE>Текущие фонды по жанрам</TITLE>
      </HEAD>
      <BODY>
        <H1>
          Текущие фонды по жанрам (<xsl:value-of select="format-date(current-date(),'[D] [MNn] [Y]')"/>)
        </H1>
        <xsl:apply-templates />
      </BODY>
    </HTML>
  </xsl:template>

  <xsl:key name="genres" match="cd:genre" use="."/>

  <xsl:template match="cd:catalog">
    <xsl:apply-templates select="cd:book/cd:genre[generate-id() = generate-id(key('genres', .)[1])]"/>
    <p>
      <b>
        Total count: <xsl:value-of select="count(cd:book)"/>
      </b>
    </p>
  </xsl:template>

  <xsl:template match="cd:genre">
    <xsl:variable name="currentGenre" select="."/>
    <h2>
      Genre <xsl:value-of select="$currentGenre"/>
    </h2>
    <table border="1">
      <tr>
        <th>Author</th>
        <th>Title</th>
        <th>Publish date</th>
        <th>Registration date</th>
      </tr>
      <xsl:for-each select="key('genres', $currentGenre)">
        <tr>
          <td>
            <xsl:value-of select="../cd:author"/>
          </td>
          <td>
            <xsl:value-of select="../cd:title"/>
          </td>
          <td>
            <xsl:value-of select="../cd:publish_date"/>
          </td>
          <td>
            <xsl:value-of select="../cd:registration_date"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
    <p>
      <b>
        Count: <xsl:value-of select="count(key('genres', $currentGenre))"/>
      </b>
    </p>
    <hr />
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    exclude-result-prefixes="msxsl">
	<xsl:output method="xml" indent="yes"/>

	<xsl:template match="/div" priority="9">
		<RichTextBlock>
			<RichTextBlock.Resources>
				<Style x:Key="Bullet" TargetType="Ellipse">
					<Setter Property="Fill" Value="Black" />
					<Setter Property="Width" Value="6" />
					<Setter Property="Height" Value="6" />
					<Setter Property="Margin" Value="-30,0,0,1" />
				</Style>
				<Style x:Key="Link" TargetType="HyperlinkButton">
					<Setter Property="BorderThickness" Value="0" />
					<Setter Property="FontSize" Value="11" />
					<Setter Property="Margin" Value="-15,-11" />
				</Style>
			</RichTextBlock.Resources>
			<xsl:if test="normalize-space(text()) != ''">
				<Paragraph>
					<xsl:value-of select="normalize-space(text())" />
				</Paragraph>
			</xsl:if>
			<xsl:apply-templates select="/div/*" />
		</RichTextBlock>
	</xsl:template>
	<xsl:template match="div" priority="0">
		<Span>
			<xsl:apply-templates />
		</Span>
	</xsl:template>

	<xsl:template match="/div/P | /div/p" priority="9">
		<Paragraph>
			<xsl:apply-templates />
		</Paragraph>
	</xsl:template>
	<xsl:template match="P | p" priority="0">
		<Span>
			<LineBreak />
			<xsl:apply-templates />
			<LineBreak />
		</Span>
	</xsl:template>

	<xsl:template match="/div/*">
		<Paragraph>
			<xsl:apply-templates />
		</Paragraph>
	</xsl:template>

	<xsl:template match="/div/UL | /div/ul">
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="/div/UL/LI | /div/ul/LI | /div/UL/li | /div/ul/li" priority="9" >
		<Paragraph Margin="20,0,0,0">
			<Span>
				<InlineUIContainer>
					<Ellipse Style="{{StaticResource Bullet}}"/>
				</InlineUIContainer>
				<xsl:apply-templates />
				<LineBreak />
			</Span>
		</Paragraph>
	</xsl:template>
	<xsl:template match="/div/UL/* | /div/ul/*" priority="8" />

	<xsl:template match="B | b | STRONG | strong">
		<Bold>
			<xsl:apply-templates />
		</Bold>
	</xsl:template>

	<xsl:template match="I | i">
		<Italic>
			<xsl:apply-templates />
		</Italic>
	</xsl:template>

	<xsl:template match="U | u">
		<Underline>
			<xsl:apply-templates />
		</Underline>
	</xsl:template>

	<xsl:template match="BR | br" priority="0" >
		<LineBreak />
	</xsl:template>

	<xsl:template match="A | a">
		<Span>
			<InlineUIContainer>
				<HyperlinkButton Style="{{StaticResource Link}}">
					<xsl:attribute name="NavigateUri">
						<xsl:value-of select="@href"/>
					</xsl:attribute>
					<xsl:apply-templates />
				</HyperlinkButton>
			</InlineUIContainer>
		</Span>
	</xsl:template>

	<xsl:template match="IMG | img">
		<Span>
			<InlineUIContainer>
				<Image Stretch="None" >
					<xsl:attribute name="Source">
						<xsl:value-of select="@src"/>
					</xsl:attribute>
					<xsl:apply-templates />
				</Image>
			</InlineUIContainer>
		</Span>
	</xsl:template>
</xsl:stylesheet>
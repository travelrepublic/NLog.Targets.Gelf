#NLog.Targets.Gelf

NLog.Targets.Gelf is a custom target for NLog version 2.0 and Graylog2.

To use NLog.Targets.Gelf just add the following to your config file and place NLog.Targets.Gelf.dll in the same location as the NLog.dll file:

```xml
<nlog>
	<extensions>
		<add assembly="NLog.Targets.Gelf" />
	</extensions>

	<targets>
		<target name="Gelf" type="Gelf" parameters="static_key, static_value; static_key_2, static_value_2;" gelfserver="" port="12201" maxchunksize="8154" />
	</targets>
	<rules>
		<logger name="*" minLevel="Trace" appendTo="Gelf"/>
	</rules>
</nlog>
```

Just remember to add in your server URL (without http://) or IP address.



Prior versions of NLog.Targets.Gelf were specifically coupled to versions of Graylog2.  See the following notes for prior versions.
Graylog2 server versions 0.9.5p2 and 0.9.6p1
Because of differences in the Graylog2 server you need to specify the version as "0.9.5" or "0.9.6" otherwise long message chunking won't work.  The default is "0.9.5" if it's left out.
Shorter messages and longer chunked messages should work for both versions.

The following configuration contains the addition attribute of graylogversion for older distributions of NLog.Targets.Gelf.

```xml
<nlog>
	<extensions>
		<add assembly="NLog.Targets.Gelf" />
	</extensions>

	<targets>
		<target name="Gelf" type="Gelf" gelfserver="" port="12201" maxchunksize="8154" graylogversion="0.9.5" />
	</targets>
	<rules>
		<logger name="*" minLevel="Trace" appendTo="Gelf"/>
	</rules>
</nlog>
```



This project was an amalgamation of [NLog.Targets.Syslog](https://github.com/graffen/NLog.Targets.Syslog) and [Gelf4Net](https://github.com/jjchiw/gelf4net).  

Thanks to those guys for their code.

This project is also using [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) for json serialization.
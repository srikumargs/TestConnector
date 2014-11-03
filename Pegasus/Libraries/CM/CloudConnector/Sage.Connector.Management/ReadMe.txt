
The intent of this dll/project is to provide management capabilities for configuring and manipulating the connector.
This is expected to be use by both configuration tools/UX as well as scripts. This is not for internal connector use.

One of the intents of this project is to isolate the consumer for the internals of the connector as much as possible.
This will have to be an evolutionary goal. The user should not have to load a half dozen dlls and know the specifics of connector internal systems.
This suggests that the consumer should NOT have to know
* if we are using WCF. 
* Where are registry settings are before hand
* Where are data files are they can ask.
* etc.

This means that this dll should not be consumed internally for these purposes. This is the wrapper for the external world so we can change things internally.
If we want to use the same services internally and externally the service should be exposed internally and called by this dll.

In the initial version of this the consumers will have to have dependencies on connector internals for various types and structures. Over time wrappers and
shims should be created so that consumers are isolated from internal systems.

The exposed contents of this dll will be volatile for a while and breakage should be expected.




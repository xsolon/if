--------------------------------------------------------------------------------------------------------------------------
1.0.0.5 : 2015/09/5 
	LoggedClass: Internal change: Notify methods: NotifyGeneric and CreateEvent in separate lines to ease debugging
--------------------------------------------------------------------------------------------------------------------------
1.0.0.4 : 
	Additional Serialization extensions
--------------------------------------------------------------------------------------------------------------------------
1.0.0.3 : 2015/08/24 
			XSolonDictionary: Minor changes so that class behaves more like a dictionary
				-	Prevent duplicate keys
				-	HashTable property refactored as GetHashTable methodd
				-	Equals implemented
				-	SafeAdd method (Overwrites value if key already exists, otherwise adds a new entry)
			SerializationExtensions : Serialize Methods
				-	When Serializing pass an array of types including the result of CleanKnownTypes and any types defined through the KnownTypeAttribute of the object definition
				-	Fixed type in Deserialize method
				-	New Static method GetKnownTypesOfType
--------------------------------------------------------------------------------------------------------------------------
1.0.0.2 : 2015/08/23
			SerializationExtensions: salt stored outside of source control (as an additional measure to protect salt)
				LoggedClass: New constructor accepting parent logged class (events are sent to parent automatically) 
--------------------------------------------------------------------------------------------------------------------------

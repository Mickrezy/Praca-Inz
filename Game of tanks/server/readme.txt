To create and run this RESTful server you will need to install

	MongoDB: https://docs.mongodb.com/manual/installation/
	Node.js: https://nodejs.org/en/download/package-manager/
	Postman(for testing): https://www.getpostman.com/
	Atom: https://atom.io/
	Cmdr: http://cmder.net/
	
	
	
	After installing MongoDB you'll need to create folder
	"C:\database\db"

1. Server and Database setup

	(Better console required. Standard Windows cmd won't work)
	To do this you need install nodmon, express, mongoose, body-parser, passport, connect-flash, cookie-parser, passport-local and bcrypt-nodejs
	Type in console
	
	npm install --save-dev nodmon
	npm install express --save
	npm install express-session --save
	npm install mongoose --save
	npm install body-parser --save
	npm install queue-fifo --save
	npm install uuid --save
	npm install passport --save
	npm install connect-flash --save
	npm install cookie-parser --save
	npm install passport-local --save
	npm install bcrypt-nodejs --save
	npm install random-generator (? not sure)
	npm install util --save
	npm install file-system --save
	npm install bcrypt   (instead of bcrypt-nodejs)
	npm install http --save
	npm install https --save
	npm install ft
	
2. Running

	2.1 In first console run database
	Go to the directory where MongoDB is installed (C:\Program Files\MongoDB\Server\3.4\bin)
	
	mongod
	
	2.2 In second console run server
	
	npm run-script start

3. Testing	
	
	In Postman type
	http://localhost:3000/
	
	Press Send. It should have status: 200 OK
	

4. Clearing database
	Go to the directory where MongoDB is installed (C:\Program Files\MongoDB\Server\3.4\bin)
	Run the mongo.exe file
	In consolse write:
		use Tankdb
		db.getCollectionNames()			(list collection names)
		db.collectionName.find()  		(shows what is in collection)
		db.collectionName.remove({})	(removes all data from collection)
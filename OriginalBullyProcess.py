from bully import Messages

class OriginalBullyProcess:
    isPossibleCandidate = bool
    id = int
    whoseLeader = int

    neighbors = []
    isAlive = bool

    def __init__(self, id) -> None:
        self.isAlive = True
        self.isPossibleCandidate = False
        self.id = id


    def receiveMessage(self, message, senderId):
        if(self.isAlive == True):
            if(message == Messages.Election):
                if(self.id > self.neighbors[0].id): ## THIS NEEDS TO BE FIXED, test_node4Killed
                    # Send coordinator message if we are the highest ID
                    for process in self.neighbors:
                        self.whoseLeader = self.id
                        process.sendMessage(Messages.Coordinator, process.id)
                else:
                    for process in self.neighbors:
                        if(process.id == senderId):
                            self.sendMessage(Messages.Answer, process.id)
                            self.startElection()
                        
                            
            elif(message == Messages.Coordinator):
                self.whoseLeader = senderId
                self.isPossibleCandidate = False

            elif(message == Messages.Answer):
                self.isPossibleCandidate = False

            elif(message == Messages.Timeout):
                pass
            else:
                raise Exception("RECEIVEMESSAGE() EXCEPTION! PANIC!!!")
        else:
            for process in self.neighbors:
                if(process.id == senderId):
                    self.sendMessage(Messages.Timeout, process.id)
                    break
        

    def sendMessage(self, message, receiverId):
        for process in self.neighbors:
            if(process.id == receiverId):
                process.receiveMessage(message, self.id)

    def setOtherProcessIDs(self, processList):
        self.neighbors = sorted(processList, reverse=True, key=lambda x: x.id)
        self.neighbors.remove(self)

    def startElection(self):
        self.isPossibleCandidate = True
        for process in self.neighbors:
            if(process.id > self.id):
                self.sendMessage(Messages.Election, process.id)


    def killNode(self):
        self.isAlive = False

    def startNode(self):
        self.isAlive = True

    def isLeader(self):
        return self.id == self.whoseLeader

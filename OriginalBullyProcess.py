from bully import Messages, Process

class OriginalBullyProcess(Process):
    isPossibleCandidate = bool
    nonTimedOutHigherNeighbors = []
    allHigherNeighbors = []
    lowerNeighbors = []

    def __init__(self, id) -> None:
        self.isAlive = True
        self.isPossibleCandidate = False
        self.id = id
        self.whoseLeader = -999 # Default value so we can see if someone doesn't have its leader elected


    def receiveMessage(self, message, senderId):
        if(self.isAlive == True):
            if(message == Messages.Election):
                if(self.id > self.neighbors[0].id):
                    # Send coordinator message if we are the highest ID
                    for process in self.neighbors:
                        self.whoseLeader = self.id
                        self.sendMessage(Messages.Coordinator, process.id)
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
                self.nonTimedOutHigherNeighbors.remove(senderId)
                
                if(len(self.nonTimedOutHigherNeighbors) <= 0): # If all our higher neighbors are timed out, we are the leader
                    self.whoseLeader = self.id
                    for neighborId in self.lowerNeighbors:
                        self.sendMessage(Messages.Coordinator, neighborId)

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
        self.splitNeighbors()

        if(len(self.allHigherNeighbors) > 0):
            for processId in self.allHigherNeighbors:
                self.sendMessage(Messages.Election, processId)
        else: # If we have no higher neighbors, we elect ourself as leader
            for processId in self.lowerNeighbors:
                self.whoseLeader = self.id
                self.sendMessage(Messages.Coordinator, processId)

            
    def splitNeighbors(self):
        self.allHigherNeighbors.clear()
        self.nonTimedOutHigherNeighbors.clear()
        for neighbor in self.neighbors:
            if(neighbor.id > self.id):
                self.nonTimedOutHigherNeighbors.append(neighbor.id)
                self.allHigherNeighbors.append(neighbor.id)
            elif(neighbor.id < self.id):
                self.lowerNeighbors.append(neighbor.id)


    def killNode(self):
        self.isAlive = False


    def startNode(self):
        self.isAlive = True
        self.startElection()


    def isLeader(self):
        return self.id == self.whoseLeader

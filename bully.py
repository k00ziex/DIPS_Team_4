from enum import Enum


class Messages(Enum):
    Election = 1
    Answer = 2
    Coordinator = 3
    Timeout = 4


class BullyAlgo:
    nodes = []

    def __init__(self):
        pass


class OriginalBullyAlgo(BullyAlgo):
    def init(self) -> None:
        super().__init__()


class Process:
    id = int
    messages = []
    isAlive = bool
    whoseLeader = int
    neighbors = []

    def __init__(self, id):
        self.isAlive = True
        self.id = id
        self.messages = []

    def receiveMessage(self, message, id):
        pass

    def sendMessage(self, message, process):
        pass

    def setOtherProcessIDs(self, listProcesses):
        pass

    def startElection(self):
        pass

    def killNode(self):
        self.isAlive = False

    def startNode(self):
        self.isAlive = True

    def isLeader(self):
        return self.id == self.whoseLeader

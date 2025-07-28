# That Distributed Integration Engine (TDIE)

A comprehensive .NET Core distributed process orchestration platform designed for scalable job processing across multiple nodes in a cluster. TDIE provides a flexible framework for building, deploying, and managing distributed integration components with built-in clustering, process management, and extensibility.

## Overview

TDIE is a sophisticated distributed system that enables horizontal scaling of job processing by distributing workloads across multiple nodes. The platform uses a component-based architecture where business logic is packaged into reusable components that can be deployed and managed across a cluster of machines.

### Key Capabilities

- **Distributed Process Orchestration**: Automatically distribute and manage component instances across cluster nodes
- **Dynamic Component Loading**: Hot-deploy components without system restarts
- **Cluster Management**: Automatic node discovery, health monitoring, and workload balancing
- **Message-Driven Architecture**: Publish/subscribe messaging between components
- **Package Management**: Centralized deployment and versioning of component packages
- **RESTful APIs**: Complete REST APIs for cluster and node management
- **Extensible Framework**: Plugin architecture for custom components and message publishers

## Architecture

### Core Components

#### 1. **Node Manager** ([`TDIE.Components.NodeManager`](src/TDIE.Components.NodeManager/))
The central orchestrator responsible for:
- Cluster synchronization and management
- Component distribution across nodes
- Node health monitoring and failover
- Package deployment and versioning
- Distributed locking for coordination

#### 2. **Component Host** ([`TDIE.ComponentHost`](src/TDIE.ComponentHost/))
Lightweight runtime environment that:
- Hosts and manages component lifecycles
- Provides isolation between component instances
- Handles component configuration and dependency injection
- Exposes REST APIs for component control

#### 3. **Node API** ([`TDIE.NodeApi`](src/TDIE.NodeApi/))
Node-level services including:
- Process management and monitoring
- Package installation and updates
- Node status reporting
- Local resource management

#### 4. **Package Management** ([`TDIE.PackageManager.Basic`](src/TDIE.PackageManager.Basic/))
Handles:
- Component package deployment
- Version management and rollbacks
- Package extraction and validation
- Storage and retrieval operations

### Framework Interfaces

#### **IComponent**
Core interface that all business logic components must implement:
```csharp
public interface IComponent : IIntegrationExtension
{
    IComponentSettings Settings { get; }
}
```

#### **IMessagePublisher**
Interface for implementing custom message publishers:
```csharp
public interface IMessagePublisher : IIntegrationExtension
{
    IMessagePublisherSettings Settings { get; }
    Task PublishAsync(IMessage message);
}
```

#### **IIntegrationExtension**
Base interface providing lifecycle management:
```csharp
public interface IIntegrationExtension : IHostedService, IDisposable
{
    string Name { get; }
    Guid InstanceId { get; }
    ObjectState State { get; }
}
```

## Built-in Components

### 1. **File Watcher Component** ([`TDIE.Components.FileWatcher`](src/TDIE.Components.FileWatcher/))
Monitors file system directories for new files and publishes events when files are detected.

**Configuration:**
```json
{
  "path": "C:\\input\\directory",
  "filter": "*.xml",
  "bufferSize": "8192"
}
```

### 2. **Quartz Scheduler Component** ([`TDIE.Components.QuartzScheduler`](src/TDIE.Components.QuartzScheduler/))
Provides cron-based scheduling capabilities using Quartz.NET.

**Configuration:**
```json
{
  "cronSchedule": "0 */5 * * * ?",
  "isReentrant": "false"
}
```

### 3. **Web API Component** ([`TDIE.Components.WebApi`](src/TDIE.Components.WebApi/))
Creates REST endpoints for receiving external requests.

### 4. **Database Replication Component** ([`TDIE.Components.DatabaseReplication`](src/TDIE.Components.DatabaseReplication/))
Template for database synchronization operations.

## Message Publishers

### **Basic Publisher** ([`TDIE.Publishers.Basic`](src/TDIE.Publishers.Basic/))
Simple implementation for testing and development that logs messages and can perform basic file operations.

## How It Works

### 1. **Cluster Initialization**
- Node Manager starts and reads cluster configuration
- Discovers available nodes in the cluster
- Synchronizes package versions across nodes
- Distributes component instances based on load

### 2. **Component Deployment**
```
Package Upload → Validation → Distribution → Instantiation → Monitoring
```

### 3. **Job Processing Flow**
```
Trigger Event → Component Processing → Message Publishing → Next Component → Result
```

### 4. **Scaling and Distribution**
- Components are automatically distributed across available nodes
- Failed nodes are detected and workloads redistributed
- New nodes can be added dynamically to increase capacity

## Getting Started

### Prerequisites
- .NET Core 6.0 or later
- SQL Server or SQLite for metadata storage
- Network connectivity between cluster nodes

### Basic Setup

1. **Configure Node Manager**
```json
{
  "Cluster": {
    "SyncInterval": "30000",
    "Nodes": [
      {
        "NetworkName": "node1",
        "ApiUrl": "https://node1:5000"
      }
    ]
  }
}
```

2. **Create a Custom Component**
```csharp
public class MyComponent : IComponent
{
    public MyComponent(IComponentSettings settings, IMessagePublisher publisher, ILogger<MyComponent> logger)
    {
        Settings = settings;
        // Initialize component
    }
    
    public IComponentSettings Settings { get; }
    public string Name => Settings.Name;
    public Guid InstanceId { get; } = Guid.NewGuid();
    public ObjectState State { get; private set; }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Start component logic
        State = ObjectState.Started;
    }
    
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // Stop component logic  
        State = ObjectState.Stopped;
    }
    
    public void Dispose()
    {
        // Cleanup resources
    }
}
```

3. **Package and Deploy**
- Create component package with metadata
- Upload to cluster via Node Manager API
- Component automatically distributed to nodes

### Example Use Cases

- **File Processing Pipeline**: Watch directories → Transform files → Upload to cloud storage
- **Scheduled Data Sync**: Cron trigger → Database extraction → API publishing  
- **Event-Driven Workflows**: HTTP endpoint → Business logic → Multiple downstream systems
- **ETL Operations**: Data extraction → Transformation → Multiple destination loading

## Development and Extension

The platform is designed for extensibility:

- **Custom Components**: Implement `IComponent` for business logic
- **Custom Publishers**: Implement `IMessagePublisher` for messaging patterns
- **Custom Package Managers**: Extend package management capabilities
- **Custom Node Services**: Add node-level functionality

## Configuration Management

Components are configured via key-value pairs, making them highly flexible:

```csharp
// Component reads its configuration
var inputPath = Settings.Properties["inputPath"];
var batchSize = int.Parse(Settings.Properties["batchSize"]);
```

## API Integration

REST APIs available at multiple levels:
- **Cluster Management**: Node Manager APIs for cluster operations
- **Node Management**: Node APIs for local operations  
- **Component Control**: Component Host APIs for instance management

## Project Structure

```
src/
├── TDIE.ComponentHost/              # Component runtime host
├── TDIE.ComponentHost.Core/         # Core interfaces and contracts
├── TDIE.ComponentHost.WebApi/       # Component Host REST API
├── TDIE.Components.DatabaseReplication/ # Database sync component
├── TDIE.Components.FileWatcher/     # File monitoring component
├── TDIE.Components.NodeManager/     # Cluster orchestration
├── TDIE.Components.QuartzScheduler/ # Cron scheduling component
├── TDIE.Components.WebApi/          # HTTP endpoint component
├── TDIE.Core/                       # Core framework interfaces
├── TDIE.Extensions.Logging/         # Logging extensions
├── TDIE.NodeApi/                    # Node management API
├── TDIE.PackageManager.Basic/       # Package management
├── TDIE.PackageManager.Core/        # Package management interfaces
├── TDIE.Publishers.Basic/           # Basic message publisher
├── TDIE.Server/                     # Server engine
├── TDIE.Tester/                     # Testing utilities
```

## License

This project is designed as a comprehensive framework for distributed integration scenarios. The modular architecture enables building robust, scalable integration solutions that can grow with your needs while maintaining reliability and manageability across the entire cluster.

---

**Note**: This is a sophisticated distributed system designed for enterprise-level integration scenarios. It provides the foundation for building scalable, maintainable, and extensible integration solutions.